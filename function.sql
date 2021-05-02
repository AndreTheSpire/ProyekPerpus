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