public interface ITwilmeetService
{
    Task<Object> Get();
    Task<Object> GetById(string id);
    Task<Object> Post(CreateTwilmeetDto items, string idUser);
    Task<Object> PostBuy(CreateBuyTwilmeetDto items);
    Task<Object> PostApproval(string idUser);
    Task<Object> PostDecline(string idUser);

    Task<Object> GetMemberWebinar(string idUser);

    Task<Object> Put(string id, CreateTwilmeetDto items);
    Task<Object> Delete(string id);
}