namespace DietApp.Application.Interfaces;

public interface IConverter<EntityType, DTOType> 
    where EntityType : class, IEntity                                                
    where DTOType : class, IResponse
{
    DTOType ToResponse(EntityType entityType);

    EntityType ToEntity(DTOType type);
}
