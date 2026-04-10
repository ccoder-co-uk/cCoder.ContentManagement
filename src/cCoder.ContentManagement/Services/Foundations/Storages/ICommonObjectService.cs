using CommonObject = cCoder.Data.Models.CommonObject;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

public interface ICommonObjectService
{
    CommonObject Get(int id, bool ignoreFilters = false);

    IQueryable<CommonObject> GetAll(bool ignoreFilters = false);

    ValueTask<CommonObject> AddAsync(CommonObject commonObject);

    ValueTask<CommonObject> UpdateAsync(CommonObject commonObject);

    ValueTask DeleteAsync(int id);
}
