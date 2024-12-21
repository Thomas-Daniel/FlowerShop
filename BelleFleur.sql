DROP DATABASE IF EXISTS fleuriste;
CREATE DATABASE IF NOT EXISTS fleuriste;
USE fleuriste;


-- Création des tables


drop table if exists magasin;
CREATE TABLE IF NOT EXISTS magasin(
	idM int primary key not null AUTO_INCREMENT,
    nom varchar(40) not null,
    adresse varchar(40) not null
);

DROP TABLE IF EXISTS depot;
CREATE TABLE IF NOT EXISTS depot (
	idDepot int not null primary key AUTO_INCREMENT,
    adresse varchar(50) not null,
    capacite int
);

DROP TABLE IF EXISTS client;
CREATE TABLE IF NOT EXISTS client(
	idC int primary key not null AUTO_INCREMENT,
    nom VARCHAR(40) not null,
    prenom VARCHAR(40) not null,
    tel varchar(15) not null,
    mail varchar(50) not null,
    passwd varchar(50) not null,
    adresseFact VARCHAR(50) not null,
    cb varchar(50),
    fidelite varchar(8),
    idM int, FOREIGN KEY (idM) REFERENCES magasin(idM)
);

DROP TABLE IF EXISTS bouquet_perso;
CREATE TABLE IF NOT EXISTS bouquet_perso (
  idBouqPerso INT PRIMARY KEY not null AUTO_INCREMENT,
  description varchar(500),
  prixMax decimal,
  isIndecis bool not null
);

DROP TABLE IF EXISTS bouquet_standard;
CREATE TABLE IF NOT EXISTS bouquet_standard (
  idBouqStand INT PRIMARY KEY not null AUTO_INCREMENT,
  nom varchar(50) not null,
  composition varchar(500) not null,
  prix decimal not null,
  categorie varchar(50) not null
);

DROP TABLE IF EXISTS commande;
CREATE TABLE IF NOT EXISTS commande (
	idCom int primary key not null AUTO_INCREMENT,
	dateCom date not null,
	adresseLiv varchar(50) not null,
	dateLiv date not null,
    message VARCHAR(500),
	statut varchar(50) not null,    
    typeCom varchar(50) not null,
    idBouqPerso INT, FOREIGN KEY (idBouqPerso) REFERENCES bouquet_perso(idBouqPerso),
    idBouqStand INT, FOREIGN KEY (idBouqStand) REFERENCES bouquet_standard(idBouqStand),
	idC INT not null, FOREIGN KEY (idC) REFERENCES client(idC)
);

DROP TABLE IF EXISTS produit;
CREATE TABLE IF NOT EXISTS produit (
  idP INT not null primary key AUTO_INCREMENT,
  typeP varchar(50) not null,
  nom varchar(50) not null,
  prix decimal not null,
  dispo varchar(50) not null,
  stock int not null,
  idM int not null, foreign key (idM) references magasin(idM)
);

DROP TABLE IF EXISTS item_bouquet;
CREATE TABLE IF NOT EXISTS item_bouquet (
	idBouqPerso INT not null,
	idP INT not null,
    quantite INT not null,
	primary key(idBouqPerso, idP),
    foreign key (idBouqPerso) references bouquet_perso(idBouqPerso),
    foreign key (idP) references produit(idP)
);


-- Insertion des tuples


/* Magasin */

insert into magasin(idM, nom, adresse) values (1, "Floriflore", "17 rue de la Montée, Paris");
insert into magasin(idM, nom, adresse) values (2, "Floralia", "9 avenue du pont, Reims");

/* Client */

insert into client(idC, prenom, nom, tel, mail, passwd, adresseFact, cb, fidelite, idM) values (1, "Thomas", "Daniel", "0602038799", "thomas747583@gmail.com", "Titou7415", "8 rue de la Forêt, Orléans", "8475578974651234", "Or", 2);
insert into client(idC, prenom, nom, tel, mail, passwd, adresseFact, cb, fidelite, idM) values (2, "Thomas", "Collot", "0607789415", "thomas.collot@gmail.com", "weshwesh", "12 rue de la montre, Paris", "7842759614785236", null, 1);
insert into client(idC, prenom, nom, tel, mail, passwd, adresseFact, cb, fidelite, idM) values (3, "Mathieu", "Chauve", "0478159647", "bonbon@gmail.com", "zef4za5f", "9 avenue de la courroie, Bougival", "78945125858559", "Bronze", 1);
insert into client(idC, prenom, nom, tel, mail, passwd, adresseFact, cb, fidelite, idM) values (4, "Antoine", "Crapu", "0102040507", "frerot@outlook.com", "password", "120 route de la maison, Saint-Cyr", "84484561662656", null, 1);
insert into client(idC, prenom, nom, tel, mail, passwd, adresseFact, cb, fidelite, idM) values (5, "Quoi", "Feur", "0104895674", "huim@gmail.com", "ejfiezhf", "9 rue de la route, Lyon", "79526462596829", null, 2);

/* Bouquet Perso */

insert into bouquet_perso(idBouqPerso, description, prixMax, isIndecis) values (1, "Je veux un bouquet coloré avec des paillettes", 50, True);
insert into bouquet_perso(idBouqPerso, description, prixMax, isIndecis) values (2, "Je veux 5 gerbera et 20 marguerites", null, False);
insert into bouquet_perso(idBouqPerso, description, prixMax, isIndecis) values (3, "Seulement des fleurs rouges", 34, True);
insert into bouquet_perso(idBouqPerso, description, prixMax, isIndecis) values (4, "Je veux toutes vos fleurs avec 5 de chaque", null, False);

insert into bouquet_perso(idBouqPerso, description, prixMax, isIndecis) values (5, "Seulement 10 coquelicots", null, False);
insert into bouquet_perso(idBouqPerso, description, prixMax, isIndecis) values (6, "Je veux 10 roses blanches", null, False);
insert into bouquet_perso(idBouqPerso, description, prixMax, isIndecis) values (7, "Peu importe le bouquet avec un emballage et un ruban", 70, True);
insert into bouquet_perso(idBouqPerso, description, prixMax, isIndecis) values (8, "Mettez moi de tout, c'est pour un mariage", 200, True);

/* Bouquet Standard */

insert into bouquet_standard(idBouqStand, nom, composition, prix, categorie) values (1, "Gros Merci", "Arrangement floral avec marguerites et verdure", 45, "Toute occasion");
insert into bouquet_standard(idBouqStand, nom, composition, prix, categorie) values (2, "L'amoureux", "Arrangement floral avec roses blanches et roses rouges", 65, "St-Valentin");
insert into bouquet_standard(idBouqStand, nom, composition, prix, categorie) values (3, "L'Exotique", "Arrangement floral avec ginger, oiseaux du paradis, roses et genet", 40, "Toute occasion");
insert into bouquet_standard(idBouqStand, nom, composition, prix, categorie) values (4, "Maman", "Arrangement floral avec gerbera, roses blanches, lys et alstroméria", 80, "Fête des mères");
insert into bouquet_standard(idBouqStand, nom, composition, prix, categorie) values (5, "Vive la mariée", "Arrangement floral avec lys et orchidées", 120, "Mariage");

/* Commande */

-- 2 commandes VINV
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (1, '2023-01-22', '8 rue de la Forêt, Orléans', '2023-01-24', null, 'VINV', 'Standard', null, 1, 1);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (2, '2023-11-01', "9 avenue de la courroie, Bougival", '2023-11-02', null, "VINV", "Standard", null, 2, 3);

-- 5 commandes CC
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (3, '2023-01-22', "8 rue de la Forêt, Orléans", '2023-01-27', "Maria je t'aime", "CC", "Perso", 1, null, 1);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (4, '2023-10-04', "8 rue de la Forêt, Orléans", '2023-10-08', "A nos 10 ans de mariage", "CC", "Standard", null, 4, 1);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (5, '2023-04-10', "12 rue de la montre, Paris", '2023-05-17', null, "CC", "Perso", 2, null, 2);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (6, '2023-01-29', "8 rue de la Forêt, Orléans", '2023-02-28', null, "CC", "Standard", null, 5, 1);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (7, '2023-07-07', "9 rue de la route, Lyon", '2023-09-09', null, "CC", "Standard", null, 1, 5);

-- 4 commandes CPAV
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (8, '2023-07-15', "15 rue de la Mouette, Reims", '2023-09-15', "Je t'aime", "CPAV", "Perso", 3, null, 1);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (9, '2023-02-12', "8 rue de la Forêt, Orléans", '2023-02-19', null, "CPAV", "Perso", 4, null, 1);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (10, '2023-07-25', "9 avenue de la courroie, Bougival", '2023-08-23', null, "CPAV", "Perso", 5, null, 3);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (11, '2023-09-17', "9 avenue de la courroie, Bougival", '2023-11-12', null, "CPAV", "Perso", 6, null, 3);

-- 3 commandes CAL
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (12, '2023-08-20', '8 rue de la Forêt, Orléans', '2023-08-27', 'Tu aimes mes fleurs ?', 'CAL', 'Perso', 7, null, 1);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (13, '2023-11-25', '9 rue de la route, Lyon', '2023-11-30', null, 'CAL', 'Standard', null, 2, 5);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (14, '2023-09-12', '9 avenue de la courroie, Bougival', '2023-09-25', null, 'CAL', 'Standard', null, 2, 3);

-- 3 commandes CL
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (15, '2013-08-27', '8 rue de la Forêt, Orléans', '2013-09-15', null, 'CL', 'Perso', 8, null, 1);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (16, '2022-11-12', '12 rue de la montre, Paris', '2022-11-14', 'Tu penses encore à moi ?', 'CL', 'Standard', null, 1, 2);
insert into commande(idCom, dateCom, adresseLiv, dateLiv, message, statut, typeCom, idBouqPerso, idBouqStand, idC) values (17, '2018-04-05', '9 rue de la route, Lyon', '2023-04-10', null, 'CL', 'Standard', null, 1, 5);

/* Produits */

-- Fleurs
insert into produit(idP, typeP, nom, prix, dispo, stock, idM) values (1, 'Fleur', 'Gerbera', 5, "à l'année", 200, 1);
insert into produit(idP, typeP, nom, prix, dispo, stock, idM) values (2, 'Fleur', 'Ginger', 4, "à l'année", 400, 2);
insert into produit(idP, typeP, nom, prix, dispo, stock, idM) values (3, 'Fleur', 'Glaïeul', 1, "mai à novembre", 5, 1);
insert into produit(idP, typeP, nom, prix, dispo, stock, idM) values (4, 'Fleur', 'Marguerite', 2.25, "à l'année", 150, 2);
insert into produit(idP, typeP, nom, prix, dispo, stock, idM) values (5, 'Fleur', 'Rose rouge', 2.5, "à l'année", 47, 1);

-- Accessoires
insert into produit(idP, typeP, nom, prix, dispo, stock, idM) values (6, 'Accessoire', 'Paillette', 1, "à l'année", 100, 1);
insert into produit(idP, typeP, nom, prix, dispo, stock, idM) values (7, 'Accessoire', 'Vase', 5, "à l'année", 50, 2);
insert into produit(idP, typeP, nom, prix, dispo, stock, idM) values (8, 'Accessoire', 'Ruban', 0.4, "à l'année", 170, 1);
insert into produit(idP, typeP, nom, prix, dispo, stock, idM) values (9, 'Accessoire', 'Emballage', 0.4, "à l'année", 70, 2);
insert into produit(idP, typeP, nom, prix, dispo, stock, idM) values (10, 'Accessoire', 'Plume', 0.2, "à l'année", 300, 1);

/* Item bouquet */

insert into item_bouquet(idBouqPerso, idP, quantite) values (1, 3, 48);
insert into item_bouquet(idBouqPerso, idP, quantite) values (1, 6, 2);

insert into item_bouquet(idBouqPerso, idP, quantite) values (2, 1, 5);
insert into item_bouquet(idBouqPerso, idP, quantite) values (2, 4, 20);

insert into item_bouquet(idBouqPerso, idP, quantite) values (7, 5, 20);
insert into item_bouquet(idBouqPerso, idP, quantite) values (7, 2, 4);
insert into item_bouquet(idBouqPerso, idP, quantite) values (7, 9, 1);
insert into item_bouquet(idBouqPerso, idP, quantite) values (7, 8, 1);

insert into item_bouquet(idBouqPerso, idP, quantite) values (8, 1, 3);
insert into item_bouquet(idBouqPerso, idP, quantite) values (8, 2, 3);
insert into item_bouquet(idBouqPerso, idP, quantite) values (8, 3, 3);
insert into item_bouquet(idBouqPerso, idP, quantite) values (8, 4, 3);
insert into item_bouquet(idBouqPerso, idP, quantite) values (8, 5, 3);
insert into item_bouquet(idBouqPerso, idP, quantite) values (8, 8, 1);
