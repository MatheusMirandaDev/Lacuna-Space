# ?? Luma Space - Desafio T�cnico de Sincroniza��o de Rel�gios

Projeto desenvolvido em **C# .NET** como parte do **desafio t�cnico da Lacuna Space**, que consiste em sincronizar o rel�gio local com sondas espaciais atrav�s de uma API p�blica, simulando comunica��o interplanet�ria com precis�o temporal.

---

## ?? Sobre o Desafio

A Lacuna Space desenvolveu o **Luma**, um sistema que exige sincroniza��o precisa de rel�gios para monitoramento de sondas espalhadas pelo Sistema Solar.

O objetivo deste desafio � implementar um cliente que:

- Se autentica na API.
- Lista sondas dispon�veis.
- Sincroniza o rel�gio local com as sondas.
- Processa jobs fornecendo timestamps corretos e compensando atrasos na comunica��o (lat�ncia).

---

## ? Funcionalidades Implementadas

- ?? **Autentica��o:** Obten��o do token de acesso via API `/start`.
- ?? **Listagem de Sondas:** Busca informa��es como ID, nome e formato de timestamp.
- ? **Sincroniza��o de Rel�gios:** C�lculo de *offset* e *round-trip* para cada sonda.
- ?? **Manipula��o de Timestamps:** Suporte aos formatos `Iso8601`, `Ticks`, `TicksBinary` e `TicksBinaryBigEndian`.
- ?? **Processamento de Jobs:** Realiza a sincroniza��o e envia os dados corretos para a API `/check`.
- ?? **Tratamento Robusto de Erros:** Reinicia o processo em caso de `Fail`, `Unauthorized` ou erros na comunica��o.

---

## ?? Algoritmo de Sincroniza��o

Utiliza quatro timestamps para o c�lculo:

- `t0`: Cliente antes da requisi��o.
- `t1`: Servidor ao receber a requisi��o.
- `t2`: Servidor antes de enviar a resposta.
- `t3`: Cliente ao receber a resposta.

**F�rmulas:**

- **Offset (?):**  
  *( (t1 - t0) + (t2 - t3) ) / 2*

- **Round-trip (?):**  
  *(t3 - t0) - (t2 - t1)*

---

## ?? Detalhes da API

- ?? **Base URL:** `https://luma.lacuna.cc/`
- ?? **Content-Type:** `application/json`
- ?? **Autoriza��o:** Token Bearer (`Authorization`) � v�lido por **2 minutos**.

### ?? Endpoints utilizados:

| M�todo | Endpoint               | Descri��o                            |
|--------|-------------------------|---------------------------------------|
| POST   | `/api/start`            | Inicia o teste e retorna o token     |
| GET    | `/api/probe`            | Lista sondas                         |
| POST   | `/api/probe/{id}/sync`  | Faz a sincroniza��o com uma sonda    |
| GET    | `/api/job/take`         | Recupera um job                      |
| POST   | `/api/job/{id}/check`   | Submete o resultado do job           |

---

## Como Rodar o Projeto

### Pr�-requisitos:
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Passos:

```bash
	# Clone o reposit�rio
	git clone https://github.com/seu-usuario/luma-space.git

	# Acesse o diret�rio
	cd luma-space

	# Restaure as depend�ncias
	dotnet restore

	# Execute o projeto
	dotnet run
```

---

## ????? Autor

Matheus Miranda