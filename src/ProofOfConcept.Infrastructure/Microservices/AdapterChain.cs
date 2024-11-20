namespace ProofOfConcept.Infrastructure.Microservices;

public class AdapterChain<T> where T : IAdapter
{
    public readonly T Chain;

    public AdapterChain(List<T> adapterBases)
    {
        if (!adapterBases.Any()) throw new ArgumentException(null, nameof(adapterBases));

        Chain = adapterBases[0];

        for (var i = 0; i < adapterBases.Count; i++)
        {
            if (i + 1 >= adapterBases.Count)
            {
                return;
            }

            adapterBases[i].SetNext(adapterBases[i + 1]);
        }
    }
}