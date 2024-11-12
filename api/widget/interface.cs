public interface IWidgetService
{
    Task<Object> Get(string Username);
    Task<Object> GetUser(string ID);
    Task<Object> DeleteItem(string ID);
    Task<Object> PostNewPos(List<UpdatePos> item);
    Task<Object> AddText(string ID, CreateText item);
    Task<Object> AddLink(string ID, CreateLink item);
    Task<Object> AddImage(string ID, CreateImage item);
    Task<Object> AddVideo(string ID, CreateImage item);
    Task<Object> AddBlog(string ID, CreateContent item);
    Task<Object> AddMap(string ID, CreateMap item);
    Task<Object> AddContact(string ID, CreateContact item);
    Task<Object> AddCarousel(string ID, CreateCarausel item);
    Task<Object> ChangeWidth(string ID, ChangeWidthDto item);
    Task<Object> AddProfile(string ID, CreateProfiles item);
}