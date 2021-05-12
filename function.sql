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

--function untuk mengetahui apakah masih aktif atau tidak
create or replace function cekValidPremium(p_id number)
return number
is
    s_user users%rowtype;
    c_pernahBeli number(10);
    var_waktu number(10);
    coba number(10);
begin
    -- 0 false, 1 true
    select * into s_user from users where id = p_id;
    select count(*) into c_pernahBeli from pembelian_premium where id_user = p_id and status = 1;
    if c_pernahBeli = 0 then 
    return 0; --false
    end if;
    --mencari kapan terakhir beli
    for i in (
        select * from pembelian_premium 
        where id_user = p_id 
        order by created_at desc
    ) loop
        --apakah masih aktif ?
        --mengambil berapa hari waktu aktif
        select waktu * 30 into var_waktu from premium where i.id_premium = id;
        --membandingkan waktu
        select sysdate - created_at diff into coba from pembelian_premium where id_user = p_id and rownum = 1 order by created_at desc;
        DBMS_OUTPUT.PUT_LINE(var_waktu);
        if  var_waktu >= coba then
        return 1;
        else 
        return 0;
        end if;
    end loop;
end;
/
show err;

select cekValidPremium(1) from dual;

create or replace function cekValidPremiumKembalikan(p_id number,tanggal_pinjam date)
return number
is
    s_user users%rowtype;
    c_pernahBeli number(10);
    var_waktu number(10);
    coba number(10);
begin
    -- 0 false, 1 true
    select * into s_user from users where id = p_id;
    select count(*) into c_pernahBeli from pembelian_premium where id_user = p_id and status = 1;
    if c_pernahBeli = 0 then 
    return 0; --false
    end if;
    --mencari kapan terakhir beli
    for i in (
        select * from pembelian_premium 
        where id_user = p_id 
        order by created_at desc
    ) loop
        --apakah masih aktif ?
        --mengambil berapa hari waktu aktif
        select waktu * 30 into var_waktu from premium where i.id_premium = id;
        --membandingkan waktu
        select tanggal_pinjam - created_at diff into coba from pembelian_premium where id_user = p_id and rownum = 1 order by created_at desc;
        DBMS_OUTPUT.PUT_LINE('waktu :'||var_waktu);
        DBMS_OUTPUT.PUT_LINE('diff :'||coba);
        if  var_waktu >= coba and coba >= 0 then
        return 1;
        else 
        return 0;
        end if;
    end loop;
end;
/
show err;

select cekValidPremiumKembalikan(0,to_date('13/11/2019','dd/mm/yyyy')) from dual;