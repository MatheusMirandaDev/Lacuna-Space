# ?? Luma Space - Desafio Técnico de Sincronização de Relógios

Projeto desenvolvido em **C# .NET** como parte do **desafio técnico da Lacuna Space**, que consiste em sincronizar o relógio local com sondas espaciais através de uma API pública, simulando comunicação interplanetária com precisão temporal.

---

## ?? Sobre o Desafio

A Lacuna Space desenvolveu o **Luma**, um sistema que exige sincronização precisa de relógios para monitoramento de sondas espalhadas pelo Sistema Solar.

O objetivo deste desafio é implementar um cliente que:

- Se autentica na API.
- Lista sondas disponíveis.
- Sincroniza o relógio local com as sondas.
- Processa jobs fornecendo timestamps corretos e compensando atrasos na comunicação (latência).

---

## ? Funcionalidades Implementadas

- ?? **Autenticação:** Obtenção do token de acesso via API `/start`.
- ?? **Listagem de Sondas:** Busca informações como ID, nome e formato de timestamp.
- ? **Sincronização de Relógios:** Cálculo de *offset* e *round-trip* para cada sonda.
- ?? **Manipulação de Timestamps:** Suporte aos formatos `Iso8601`, `Ticks`, `TicksBinary` e `TicksBinaryBigEndian`.
- ?? **Processamento de Jobs:** Realiza a sincronização e envia os dados corretos para a API `/check`.
- ?? **Tratamento Robusto de Erros:** Reinicia o processo em caso de `Fail`, `Unauthorized` ou erros na comunicação.

---

## ?? Algoritmo de Sincronização

Utiliza quatro timestamps para o cálculo:

- `t0`: Cliente antes da requisição.
- `t1`: Servidor ao receber a requisição.
- `t2`: Servidor antes de enviar a resposta.
- `t3`: Cliente ao receber a resposta.

**Fórmulas:**

- **Offset (?):**  
  *( (t1 - t0) + (t2 - t3) ) / 2*

- **Round-trip (?):**  
  *(t3 - t0) - (t2 - t1)*

---

## ?? Detalhes da API

- ?? **Base URL:** `https://luma.lacuna.cc/`
- ?? **Content-Type:** `application/json`
- ?? **Autorização:** Token Bearer (`Authorization`) — válido por **2 minutos**.

### ?? Endpoints utilizados:

| Método | Endpoint               | Descrição                            |
|--------|-------------------------|---------------------------------------|
| POST   | `/api/start`            | Inicia o teste e retorna o token     |
| GET    | `/api/probe`            | Lista sondas                         |
| POST   | `/api/probe/{id}/sync`  | Faz a sincronização com uma sonda    |
| GET    | `/api/job/take`         | Recupera um job                      |
| POST   | `/api/job/{id}/check`   | Submete o resultado do job           |

---

## Como Rodar o Projeto

### Pré-requisitos:
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Passos:

```bash
	# Clone o repositório
	git clone https://github.com/seu-usuario/luma-space.git

	# Acesse o diretório
	cd luma-space

	# Restaure as dependências
	dotnet restore

	# Execute o projeto
	dotnet run
```

---

## ????? Autor

Matheus Miranda