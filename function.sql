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
select ambilIDbuku from DUAL;