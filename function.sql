--autogen_Id_Buku
create or replace function ambilIDbuku
return varchar2
is
    IDdiambil number(6);
begin
    
    select nvl(count(*),0) into IDdiambil from Buku;
    return IDdiambil;
    
end ambilIDbuku;
/
show err;

--mengatur status pembelian
create or replace procedure  set_status_pembelian(
    p_id pembelian_premium.id%type,
    p_status pembelian_premium.status%type
)
is
begin
    update pembelian_premium set status = p_status where id = p_id;
end;
/

--trigger insert pembelian premium created at pada hari pada sistem
create or replace trigger T_Insert_pembelian_premium
before insert on pembelian_premium
for each row
declare
begin
    :new.created_at := sysdate;
end;
/

--autogen id pembelian_premium
CREATE OR REPLACE FUNCTION autogen_pembelian_premium
RETURN NUMBER 
IS 
TEMP NUMBER;
BEGIN
SELECT MAX(ID) INTO TEMP FROM pembelian_premium;
RETURN TEMP + 1;
END;
/
SHOW ERR;

--autogen id user
CREATE OR REPLACE FUNCTION autogen_id_user
RETURN NUMBER 
IS 
TEMP NUMBER;
BEGIN
SELECT MAX(ID) INTO TEMP FROM users;
RETURN TEMP + 1;
END;
/
SHOW ERR;

--trigger delete buku (terpakai)
create or replace trigger T_Del_Buku
before delete on buku
for each row
begin
    delete from kategori_buku where id_buku = :old.id;
    delete from d_peminjaman where id_buku = :old.id;
end;
/
show err;