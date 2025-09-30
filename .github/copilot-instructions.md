# Instruções do Copilot para Revisões de PR

> **Objetivo geral:** orientar o Copilot em PRs para aplicar o guia do projeto e, **obrigatoriamente**, validar que **todo método** em C# possui `try { } catch { } finally { }`.

---

## Contexto do repositório

* **Stack:** ASP.NET Core (Razor Pages)
* **Domínio:** `TicTacToe.Domain`

## Guia do projeto (resumo que o Copilot deve aplicar)

* **Regras do jogo:** grade 3x3; X começa; vitória por linha/coluna/diagonal; detectar empate; impedir jogada em casa ocupada.
* **Testes:** xUnit cobrindo vitória, jogada inválida e empate; alvo **≥ 80%** de cobertura.
* **Estilo:** commits em pt-BR (conventional commits), nomes descritivos, métodos pequenos.
* **UI:** página única simples, responsiva, com `aria-label` nos botões e foco visível.
* **Pull Requests:** incluir testes e descrição do impacto.

> O Copilot deve comentar no PR sempre que qualquer item acima não for observado.

---

## Política obrigatória: todo método deve ter `try/catch/finally`

**Regra:** durante o code review, **reprovar** (solicitar mudanças) quando um **método C#** (em arquivos `.cs`) **não** encapsula sua lógica em um bloco `try { … } catch { … } finally { … }`.

### Como verificar (passo a passo)

1. Analise todos os arquivos `.cs` alterados no PR.
2. Identifique **declarações de método** em classes, serviços, handlers e `PageModel`s (inclui `public/private/protected/internal`, com ou sem `static`/`async`, com/sem generics, e `override`).
3. Para cada método, verifique se o **corpo** contém:

   * Um bloco **`try { … }`** envolvendo a **lógica do método**;
   * Pelo menos **um `catch`** com tratamento explícito (log/encapsulamento/rethrow) — **não** permitir `catch` vazio nem supressão silenciosa de exceções;
   * Um bloco **`finally`**. Pode estar vazio **apenas** se houver **comentário justificando** (ex.: `// finally requerido pela política`).
4. Se o método **não cumprir**, **comentar** no PR marcando a linha da assinatura e solicitando a adequação.

### Texto sugerido para comentário

> "**Política de exceções**: este método não está encapsulado em `try/catch/finally`. Por favor, envolva a lógica em `try`, trate exceções no(s) `catch` (sem suprimir) e inclua `finally` (comentado quando vazio)."

### Exceções mínimas (quando *não* apontar violação)

* **Auto-properties** e **records** (não possuem corpo de método).
* **Expression-bodied members** (ex.: `=> DoWork();`) **somente** quando **delegam 100%** a outro método que já cumpre a política. Ainda assim, **sugerir** converter para corpo com `try/catch/finally` quando fizer sentido.

### Boas práticas adicionais exigidas nos `catch`

* Evitar `catch (Exception)` **vazio**. Exigir log, retorno padronizado ou `throw;`.
* Evitar `throw ex;` — usar `throw;` para preservar o stack trace.
* Em métodos `async`, permitir `catch` específicos (ex.: `InvalidMoveException`) e rethrow quando necessário.

### Exemplos

**✅ Conforme**

```csharp
public async Task MakeMoveAsync(int row, int col)
{
    try
    {
        _engine.MakeMove(row, col);
        await _repo.SaveAsync(_engine.State);
    }
    catch (InvalidMoveException ex)
    {
        _logger.LogWarning(ex, "Jogada inválida em {Row},{Col}", row, col);
        throw;
    }
    finally
    {
        // finally requerido pela política (nada para liberar aqui)
    }
}
```

**❌ Não conforme (sem try/catch/finally)**

```csharp
public void MakeMove(int row, int col)
{
    _engine.MakeMove(row, col);
    _repo.Save(_engine.State);
}
```

**❌ Não conforme (catch vazio e throw ex)**

```csharp
public void Reset()
{
    try
    {
        _engine.Reset();
    }
    catch (Exception ex)
    {
        // TODO: tratar ou relançar adequadamente
        throw ex; // perde o stack original
    }
    finally { }
}
```

---

## Checklist do Copilot em PRs (usar como saída)

O Copilot deve produzir um checklist no comentário principal do review, contendo:

* [ ] Todos os métodos têm `try/catch/finally`? Liste os que **não** atendem.
* [ ] Há `catch` vazio, `throw ex;` ou supressão silenciosa?
* [ ] Testes foram adicionados/atualizados e cobertura ≥ 80%?
* [ ] UI mantém `aria-label` nos botões e foco visível?
* [ ] Commits no padrão *conventional commits* pt-BR e nomes/métodos descritivos?
* [ ] PR descreve impacto e inclui casos de teste (vitória, inválida, empate)?

Se qualquer item estiver em falta, solicitar mudanças.

---

## (Opcional) Enforçar via CI

Para complementar o review do Copilot, pode-se falhar o build quando houver método sem `try`. **Script simples em PowerShell** (heurístico):

```powershell
# scripts/check-trycatchfinally.ps1
$files = git diff --name-only origin/main...HEAD | Where-Object { $_ -like "*.cs" }
$patternMethod = '^\s*(public|private|protected|internal)\s+(static\s+)?(async\s+)?[\w\<\>\[\]]+\s+\w+\s*\([^)]*\)\s*(?:where[^\{]+)?\s*\{'
$violations = @()

foreach ($f in $files) {
  $content = Get-Content $f
  for ($i=0; $i -lt $content.Length; $i++) {
    if ($content[$i] -match $patternMethod) {
      $j = $i; $depth = 0; $hasTry = $false
      do {
        $line = $content[$j]
        if ($line -match '\{') { $depth++ }
        if ($line -match '\}') { $depth-- }
        if ($line -match '^\s*try\s*\{') { $hasTry = $true }
        $j++
      } while ($depth -gt 0 -and $j -lt $content.Length)

      if (-not $hasTry) { $violations += "$f:$($i+1)" }
    }
  }
}

if ($violations.Count -gt 0) {
  Write-Error "Métodos sem try/catch/finally:`n$($violations -join "`n")"
  exit 1
}
```

**Workflow do GitHub Actions**:

```yaml
name: policy-try-catch-finally
on: [pull_request]
jobs:
  check:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Install PowerShell
        uses: PowerShell/PowerShell@v1
      - name: Enforce try/catch/finally policy
        run: pwsh ./scripts/check-trycatchfinally.ps1
```

> Observações:
>
> * A heurística não cobre 100% dos casos (ex.: métodos multi-linha complexos), mas é útil como guarda inicial.
> * Ajuste a regex se usar *expression-bodied members* amplamente.
> * Se quiser **proibir** `finally` vazio, troque a diretriz acima para exigir limpeza/`Dispose()` e atualize os comentários sugeridos.
