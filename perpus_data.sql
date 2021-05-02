
--drop table
DROP TABLE BUKU CASCADE CONSTRAINTS PURGE;
DROP TABLE DENDA CASCADE CONSTRAINTS PURGE;
DROP TABLE D_PEMINJAMAN CASCADE CONSTRAINTS PURGE;
DROP TABLE H_PEMINJAMAN CASCADE CONSTRAINTS PURGE;
DROP TABLE PEMBELIAN_PREMIUM CASCADE CONSTRAINTS PURGE;
DROP TABLE PENGEMBALIAN CASCADE CONSTRAINTS PURGE;
DROP TABLE PREMIUM CASCADE CONSTRAINTS PURGE;
DROP TABLE USERS CASCADE CONSTRAINTS PURGE;

--create table
create table Buku(
    ID number(6) PRIMARY KEY,
    judul varchar2(255),
    author varchar2(255),
    penerbit varchar2(255),
    halaman number(10),
    status_premium number(1),
    bahasa varchar2(255)
);
create table Kategori_Buku(
    ID number(6) PRIMARY KEY,
    id_buku REFERENCES Buku(ID),
    genre varchar2(255),
);
create table users (
    ID number(6) PRIMARY KEY,
    username varchar2(255),
    password varchar2(255),
    nama varchar2(255),
    tanggal_lahir date,
    no_telp number(10)
);
create table premium(
    ID number(6) PRIMARY KEY,
    jenis varchar2(255),
    harga number(10),
    waktu number(10)
);
create table pembelian_premium(
    id number(6) PRIMARY KEY,
    id_user REFERENCES users(id),
    id_premium REFERENCES premium(id),
    status number(1),
    metode_pembayaran varchar2(255),
    created_at varchar2(255)
);
create table h_peminjaman(
    id number(6) PRIMARY KEY,
    id_user REFERENCES users(id),
    tanggal_peminjaman date,
    status_peminjaman number(1)
);
create table d_peminjaman(
    id number(6) PRIMARY KEY,
    id_h_peminjaman REFERENCES h_peminjaman(id),
    id_buku REFERENCES buku(id)
);
create table denda(
    id number(6) PRIMARY KEY,
    waktu number(10),
    denda number(10)
);
create table pengembalian(
    id number(6) PRIMARY KEY,
    id_h_peminjaman REFERENCES h_peminjaman(id),
    tanggal_pengembalian date,
    jumlah_denda number(10)
);

--insert data
--
insert into buku values(ID,'judul','author','penerbit',halaman,status_premium,bahasa)