---
applyTo: "TicTacToe.Domain/**/*.cs"
---

# Domain Layer Rules

**[DOMAIN-001]** Use records para Value Objects imutáveis (ex: Move)

**[DOMAIN-002]** Valide entradas dentro das próprias classes com exceções descritivas

**[DOMAIN-003]** Board deve validar posições (0-2) e lançar ArgumentOutOfRangeException se inválido

**[DOMAIN-004]** Board.SetCell deve lançar InvalidOperationException se célula já ocupada

**[DOMAIN-005]** GameService deve ter métodos: IsValidMove, MakeMove, CheckGameStatus, GetWinner

**[DOMAIN-006]** Detectar vitória verificando: 3 linhas, 3 colunas, 2 diagonais

**[DOMAIN-007]** NUNCA adicione dependências externas (EF, HTTP, UI, Infrastructure)

**[DOMAIN-008]** Use PascalCase para públicos, _camelCase para privados

**[DOMAIN-009]** Prefira imutabilidade e const para valores fixos