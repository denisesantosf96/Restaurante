
CREATE TABLE Restaurante	
(
  Id int not null, 
  Nome varchar(100) not NULL,
  CNPJ numeric(15) not NULL,
  Logradouro varchar(200) not NULL,
  Numero numeric(20) NULL,
  Bairro varchar(50) not NULL,
  Estado Varchar(40) not NULL,
  Pais Varchar(30) not NULL,
  CEP varchar(15) not NULL,
  Telefone varchar(13) NULL,
  HorarioFuncionamento varchar(60) NOT NULL, 
  QuantidadeMaxima varchar(80) not NULL,
  
  Constraint PK_Restaurante
  PRIMARY Key (Id) 
)



Create TABLE Mesa
(
  Id int not NULL,
  IdRestaurante Int not null,
  Localizacao varchar(100) not null,
  NumeroDaMesa Int not null,
  
  Constraint PK_Mesa
  Primary Key (Id),

  CONSTRAINT FK_Mesa_Restaurante
  FOREIGN KEY (IdRestaurante)
  REFERENCES Restaurante (Id)
  )



Create table Cliente
(
  Id int NOT NULL auto_increment,
  Nome Varchar(200) not NULL,
  CPF varchar (20) not NULL,
  RG varchar (20) NULL,
  Logradouro varchar(200) NULL,
  Numero numeric(20) NULL,
  Bairro varchar(100) NULL,
  Cidade Varchar(100) NULL,
  Estado varchar(20) NULL,
  Pais varchar(100) NULL,
  CEP varchar(20) NULL,
  Telefone varchar(15) NULL,
  Genero char not NULL,
  DataNascimento Varchar(100) NULL,
  
  CONSTRAINT PK_Cliente
  Primary key (Id)
  )

CREATE TABLE Garcom
(
  Id Int not NULL auto_increment,
  Nome Varchar(200) not NULL,
  Idade Varchar(60) not NULL,
  DataAdmissao Varchar(150) not NULL,
  
  CONSTRAINT PK_Garcom
  PRIMARY KEY (Id)
  )



Create TABLE Produto
(
  Id Int not NULL auto_increment,
  Nome Varchar(200) not NULL,
  ValorUnitario decimal (7,2) not NULL,
  
  CONSTRAINT PK_Produto
  Primary Key (Id)
  )




CREATE TABLE Pedido
(
  Id Int not NULL auto_increment,
  IdMesa Int not NULL,
  IdCliente int Not Null,
  Data Datetime not NULL,
  Qtde_itens int not NULL,
  FormaPagamento varchar(100) not NULL,
  ValorTotal decimal(10,2) not NULL,
  DataPagamento Datetime not NULL,
  
  CONSTRAINT PK_Pedido
  PRIMARY KEY (Id),
  
  CONSTRAINT FK_Pedido_Mesa
  FOREIGN KEY (IdMesa)
  REFERENCES Mesa (Id),
  
  CONSTRAINT FK_Pedido_Cliente
  FOREIGN KEY (IdCliente)
  REFERENCES Cliente (Id),
  
  CONSTRAINT FK_Pedido_Pagamento
  FOREIGN KEY (IdPagamento)
  REFERENCES Pagamento (Id)
  )



CREATE TABLE ItensPedido
(
  Id Int not NULL auto_increment,
  IdPedido int not NULL,
  IdGarcom int not NULL,
  IdProduto int not NULL,
  Quantidade numeric(5) not NULL,
  Valor decimal(7,2) not NULL,
  
  CONSTRAINT PK_ItensPedido
  Primary KEY (Id),
  
  CONSTRAINT FK_ItensPedido_Pedido
  FOREIGN key (IdPedido)
  REFERENCES Pedido (Id),
  
  CONSTRAINT FK_ItensPedido_Garcom
  FOREIGN KEY (IdGarcom)
  REFERENCES Garcom (Id),
  
  CONSTRAINT FK_ItenPedido_Produto
  FOREIGN KEY (IdProduto)
  REFERENCES Produto (Id)
  )