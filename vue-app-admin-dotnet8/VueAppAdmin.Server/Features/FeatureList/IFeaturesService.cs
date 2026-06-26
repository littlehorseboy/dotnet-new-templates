namespace VueAppAdmin.Server.Features.FeatureList;

public interface IFeaturesService
{
    IEnumerable<FeatureResponse> GetAll();
}
