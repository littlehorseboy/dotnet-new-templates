namespace VueAppAdmin.Server.Features.ExampleCategories;

public interface IExampleCategoriesService
{
    IEnumerable<ExampleCategoryResponse> GetAll();
}
