using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DietApp.Infrastructure.Db.EF.Converters;

internal class IdConverter() : ValueConverter<Id, Guid>(v => v.Value, v => new Id(v));
