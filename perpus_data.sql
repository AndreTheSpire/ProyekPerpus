--drop table
DROP TABLE BUKU CASCADE CONSTRAINTS PURGE;
DROP TABLE DENDA CASCADE CONSTRAINTS PURGE;
DROP TABLE D_PEMINJAMAN CASCADE CONSTRAINTS PURGE;
DROP TABLE H_PEMINJAMAN CASCADE CONSTRAINTS PURGE;
DROP TABLE Kategori_Buku CASCADE CONSTRAINTS PURGE;
DROP TABLE PEMBELIAN_PREMIUM CASCADE CONSTRAINTS PURGE;
DROP TABLE PENGEMBALIAN CASCADE CONSTRAINTS PURGE;
DROP TABLE PREMIUM CASCADE CONSTRAINTS PURGE;
DROP TABLE USERS CASCADE CONSTRAINTS PURGE;

--create table
create table Buku(
    ID number(6) PRIMARY KEY,
    judul varchar2(100),
    author varchar2(100),
    penerbit varchar2(100),
    halaman number(10),
    status_premium number(1),
    bahasa varchar2(100),
    status_delete number(1)
);
create table Kategori_Buku(
    ID number(6) PRIMARY KEY,
    id_buku REFERENCES Buku(id),
    genre varchar2(100)
);
create table users (
    ID number(6) PRIMARY KEY,
    username varchar2(100),
    password varchar2(100),
    nama varchar2(100),
    tanggal_lahir date,
    no_telp varchar2(15),
	status_delete number(1)
);
create table premium(
    ID number(6) PRIMARY KEY,
    jenis varchar2(100),
    harga number(10),
    waktu number(10)
);
create table pembelian_premium(
    id number(6) PRIMARY KEY,
    id_user REFERENCES users(id),
    id_premium REFERENCES premium(id),
    status number(1),
    metode_pembayaran varchar2(100),
    created_at date
);
create table h_peminjaman(
    id number(6) PRIMARY KEY,
    id_user REFERENCES users(id),
    tanggal_peminjaman date
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
--insert into buku values(ID,'judul','author','penerbit',halaman,status_premium,'bahasa');
--status premium (0 free, 1 premium)
--status delete (0 belum, 1 sudah)
insert into buku values(0,'Bikin mading lebih keren','Pramana Sukmajati','Info komputer',97,0,'Indonesia',0);
insert into buku values(1,'Ice breaker','Adi Soenarno','Penerbit Andi',56,0,'Inggris',0);
insert into buku values(2,'Resign','Susan Piver','Gramedia',110,0,'Indonesia',0);
insert into buku values(3,'Seni berbicara','Larry king','Gramedia',78,1,'Inggris',0);
insert into buku values(4,'Mencari Jodoh','Samuel','Penerbit Andi',8,1,'Indonesia',0);
insert into buku values(5,'Dont sweat the small stuff for teens','Richard Carlson','Gramedia',20,0,'Inggris',0);
insert into buku values(6,'Crayon sinchan 46','Yoshito usui','Gramedia',24,0,'Indonesia',0);
insert into buku values(7,'Crayon sinchan 25','Yoshito usui','Gramedia',26,0,'Indonesia',0);
insert into buku values(8,'skill with people','les giblin','Gramedia',103,1,'Inggris',0);
insert into buku values(9,'the art of dealing with people','les giblin','Gramedia',62,0,'Inggris',0);

--insert into kategori_buku values(ID,id_buku,'genre');
insert into kategori_buku values(0,0,'Educational');
insert into kategori_buku values(1,0,'Comedy');
insert into kategori_buku values(2,1,'Fiction');
insert into kategori_buku values(3,1,'Spy');
insert into kategori_buku values(4,2,'Drama');
insert into kategori_buku values(5,3,'Biography');
insert into kategori_buku values(6,3,'History');
insert into kategori_buku values(7,4,'Romance');
insert into kategori_buku values(8,4,'Comedy');
insert into kategori_buku values(9,5,'Family');
insert into kategori_buku values(10,5,'Relationship');
insert into kategori_buku values(11,6,'Comedy');
insert into kategori_buku values(12,7,'Comedy');
insert into kategori_buku values(13,8,'Economics');
insert into kategori_buku values(14,9,'Economics');

--insert into users values(id,'username','password','nama',tanggal_lahir,no_telp);
--status delete (0 belum, 1 sudah)
insert into users values(0, 'windah' , 'windah123', 'Windah Basudara', TO_DATE('16/09/2001', 'DD/MM/YYYY'), '081231234123', 0);
insert into users values(1, 'jokohtampan' , 'jokoh', 'Joko Bodo', TO_DATE('21/03/2002', 'DD/MM/YYYY'), '082948239032', 0);
insert into users values(2, 'budisakti' , 'budicepek', 'Budi Budiman', TO_DATE('30/11/2000', 'DD/MM/YYYY'), '089231238754', 0);
insert into users values(3, 'andre2' , 'andre2', 'Andre Taulany', TO_DATE('01/02/1999', 'DD/MM/YYYY'), '081895234323', 0);
insert into users values(4, 'petlick' , 'spongebob', 'Patrick Star', TO_DATE('06/09/2000', 'DD/MM/YYYY'), '085123897123', 0);
insert into users values(5, 'deankok' , 'dewakipas', 'Dean Sudahan', TO_DATE('28/04/1989', 'DD/MM/YYYY'), '082157298434', 0);
insert into users values(6, 'tessa' , 'tissue', 'Tessa Facial Tissue', TO_DATE('30/12/1990', 'DD/MM/YYYY'), '089890001234', 0);
insert into users values(7, 'kaptenvincent' , 'pesawat', 'Vincent Raditya', TO_DATE('28/01/1980', 'DD/MM/YYYY'), '081672987264', 0);

--insert into premium values(id,'jenis',harga,waktu);
--waktu dalam bulan
insert into premium values(0,'NewComer',25000,2);
insert into premium values(1,'Regular',30000,1);
insert into premium values(2,'Double',40000,2);
insert into premium values(3,'Triple',60000,3);
insert into premium values(4,'Semester',100000,6);
insert into premium values(5,'Yearly',180000,12);
insert into premium values(6,'TriYear',510000,36);
insert into premium values(7,'Permanent',750000,99);

--insert into pembelian_premium values(id,id_user, id_premium,status,metode_pembayaran,created_at);
--pembelian_premium (status: 0 pending, 1 accepted, 2 rejected)
insert into pembelian_premium values(0,0,0,1,'BCA',to_date('16/09/2019', 'DD/MM/YYYY'));
insert into pembelian_premium values(1,0,4,1,'OVO',to_date('16/12/2019', 'DD/MM/YYYY'));
insert into pembelian_premium values(2,1,1,1,'OVO',to_date('12/06/2020', 'DD/MM/YYYY'));
insert into pembelian_premium values(3,1,7,1,'BCA',to_date('01/09/2020', 'DD/MM/YYYY'));
insert into pembelian_premium values(4,4,7,2,'BCA',to_date('05/02/2019', 'DD/MM/YYYY'));
insert into pembelian_premium values(5,6,2,0,'Gopay',to_date('21/02/2020', 'DD/MM/YYYY'));
insert into pembelian_premium values(6,2,7,2,'BCA',to_date('12/06/2020', 'DD/MM/YYYY'));
insert into pembelian_premium values(7,5,3,0,'Gopay',to_date('15/12/2018', 'DD/MM/YYYY'));
insert into pembelian_premium values(8,3,0,2,'BCA',to_date('20/01/2020', 'DD/MM/YYYY'));
insert into pembelian_premium values(9,3,2,2,'OVO',to_date('11/04/2020', 'DD/MM/YYYY'));
insert into pembelian_premium values(10,3,7,2,'BCA',to_date('24/07/2020', 'DD/MM/YYYY'));


--insert h_peminjaman values(id,id_user,tanggal_peminjaman)
insert into h_peminjaman values(0, 0, TO_DATE('12/03/2018', 'DD/MM/YYYY'));
insert into h_peminjaman values(1, 7, TO_DATE('01/07/2018', 'DD/MM/YYYY'));
insert into h_peminjaman values(2, 6, TO_DATE('30/07/2018', 'DD/MM/YYYY'));
insert into h_peminjaman values(3, 4, TO_DATE('26/08/2018', 'DD/MM/YYYY'));
insert into h_peminjaman values(4, 2, TO_DATE('02/10/2018', 'DD/MM/YYYY'));
insert into h_peminjaman values(5, 2, TO_DATE('16/10/2018', 'DD/MM/YYYY'));
insert into h_peminjaman values(6, 5, TO_DATE('23/11/2018', 'DD/MM/YYYY'));
insert into h_peminjaman values(7, 1, TO_DATE('04/01/2019', 'DD/MM/YYYY'));
insert into h_peminjaman values(8, 5, TO_DATE('15/08/2019', 'DD/MM/YYYY'));


insert into h_peminjaman values(9, 3, TO_DATE('12/10/2019', 'DD/MM/YYYY'));
insert into h_peminjaman values(10, 7, TO_DATE('02/01/2020', 'DD/MM/YYYY'));
insert into h_peminjaman values(11, 6, TO_DATE('09/03/2020', 'DD/MM/YYYY'));
insert into h_peminjaman values(12, 5, TO_DATE('09/05/2020', 'DD/MM/YYYY'));
--premium joko
insert into h_peminjaman values(13, 1, TO_DATE('12/06/2020', 'DD/MM/YYYY'));
--
insert into h_peminjaman values(14, 0, TO_DATE('21/07/2020', 'DD/MM/YYYY'));
insert into h_peminjaman values(15, 3, TO_DATE('10/10/2020', 'DD/MM/YYYY'));
insert into h_peminjaman values(16, 4, TO_DATE('21/12/2020', 'DD/MM/YYYY'));
insert into h_peminjaman values(17, 2, TO_DATE('29/04/2021', 'DD/MM/YYYY'));
insert into h_peminjaman values(18, 7, TO_DATE('30/04/2021', 'DD/MM/YYYY'));


--insert d_peminjaman values(id,id_h_peminjaman,id_buku)
insert into d_peminjaman values(0,0,7);
insert into d_peminjaman values(1,0,5);
insert into d_peminjaman values(2,1,1);
insert into d_peminjaman values(3,2,7);
insert into d_peminjaman values(4,3,1);
insert into d_peminjaman values(5,3,5);
insert into d_peminjaman values(6,3,7);
insert into d_peminjaman values(7,4,2);
insert into d_peminjaman values(8,5,1);
insert into d_peminjaman values(9,6,5);
insert into d_peminjaman values(10,7,6);
insert into d_peminjaman values(11,8,5);
insert into d_peminjaman values(12,8,7);
insert into d_peminjaman values(13,9,9);
insert into d_peminjaman values(14,10,2);
insert into d_peminjaman values(15,11,7);
insert into d_peminjaman values(16,12,1);
insert into d_peminjaman values(17,12,5);
insert into d_peminjaman values(18,13,3);
insert into d_peminjaman values(19,13,4);
insert into d_peminjaman values(20,13,8);
insert into d_peminjaman values(21,14,9);
insert into d_peminjaman values(22,15,2);
insert into d_peminjaman values(23,16,1);
insert into d_peminjaman values(24,17,7);
insert into d_peminjaman values(25,18,2);


--insert into denda value(id,waktu,denda)
--waktu dalam hari 
insert into denda values(0, 0, 0);
insert into denda values(1, 1, 3000);
insert into denda values(2, 2, 3000);
insert into denda values(3, 3, 3000);
insert into denda values(4, 4, 3000);
insert into denda values(5, 5, 10000);
insert into denda values(6, 6, 10000);
insert into denda values(7, 7, 10000);
insert into denda values(8, 8, 10000);
insert into denda values(9, 9, 20000);
insert into denda values(10, 10, 20000);
insert into denda values(11, 11, 20000);
insert into denda values(12, 12, 20000);

--insert into pengembalian values(id,id_h_peminjaman,tanggal_pengembalian,jumlah_denda)

insert into pengembalian values(0, 0, TO_DATE('15/03/2018', 'DD/MM/YYYY'), 0);
insert into pengembalian values(1, 1, TO_DATE('20/07/2018', 'DD/MM/YYYY'), 20000);
insert into pengembalian values(2, 2, TO_DATE('02/08/2018', 'DD/MM/YYYY'), 0);
insert into pengembalian values(3, 3, TO_DATE('15/09/2018', 'DD/MM/YYYY'), 20000);
insert into pengembalian values(4, 4, TO_DATE('10/10/2018', 'DD/MM/YYYY'), 3000);
insert into pengembalian values(5, 5, TO_DATE('20/10/2018', 'DD/MM/YYYY'), 0);
insert into pengembalian values(6, 6, TO_DATE('30/11/2018', 'DD/MM/YYYY'), 0);
insert into pengembalian values(7, 7, TO_DATE('14/01/2019', 'DD/MM/YYYY'), 3000);
insert into pengembalian values(8, 8, TO_DATE('22/08/2019', 'DD/MM/YYYY'), 0);
insert into pengembalian values(9, 9, TO_DATE('13/10/2019', 'DD/MM/YYYY'), 0);
insert into pengembalian values(10, 10, TO_DATE('13/01/2020', 'DD/MM/YYYY'), 10000);
insert into pengembalian values(11, 11, TO_DATE('23/03/2020', 'DD/MM/YYYY'), 10000);
insert into pengembalian values(12, 12, TO_DATE('09/07/2020', 'DD/MM/YYYY'), 0);
insert into pengembalian values(13, 14, TO_DATE('28/07/2020', 'DD/MM/YYYY'), 0);
insert into pengembalian values(14, 13, TO_DATE('30/07/2020', 'DD/MM/YYYY'), 26000);
insert into pengembalian values(15, 15, TO_DATE('13/10/2020', 'DD/MM/YYYY'), 0);
insert into pengembalian values(16, 16, TO_DATE('29/12/2020', 'DD/MM/YYYY'), 3000);
insert into pengembalian values(17, 18, TO_DATE('03/05/2021', 'DD/MM/YYYY'), 0);
insert into pengembalian values(18, 17, TO_DATE('13/05/2021', 'DD/MM/YYYY'), 10000);

commit;