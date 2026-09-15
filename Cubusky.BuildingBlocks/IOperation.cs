namespace Cubusky.BuildingBlocks;

public interface IOperation<in TConformance>
    where TConformance : class;