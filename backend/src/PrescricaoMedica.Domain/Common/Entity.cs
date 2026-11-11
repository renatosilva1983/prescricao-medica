namespace PrescricaoMedica.Domain.Common;

/// <summary>Base das entidades: identidade numérica gerada pelo banco.</summary>
public abstract class Entity
{
    public int Id { get; protected set; }
}
