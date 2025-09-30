---
applyTo: "TicTacToe.Tests/**/*.cs"
---

# Tests Layer Rules

**[TEST-001]** Use nomenclatura: `MethodName_Scenario_ExpectedResult`

**[TEST-002]** Use estrutura AAA com comentários: // Arrange, // Act, // Assert

**[TEST-003]** Use [Theory] com [InlineData] para testar múltiplos valores

**[TEST-004]** Teste todas as vitórias: 3 linhas + 3 colunas + 2 diagonais

**[TEST-005]** Teste validações: posições fora dos limites, células ocupadas

**[TEST-006]** Teste empate com tabuleiro completo sem vencedor

**[TEST-007]** Use Assert.Throws<TException>() para verificar exceções

**[TEST-008]** Cada teste deve ser independente (sem dependência de ordem)

**[TEST-009]** NUNCA teste o framework, apenas seu código