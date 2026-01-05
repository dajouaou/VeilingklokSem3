export default function LiveKlok({ lot }) {
    if (!lot) return <p>Geen actief product</p>;

    return (
        <div className="card shadow-sm border-0 mb-4">
            <div className="card-body">
                <h3 className="h5">
                    {lot.soort} – resterend {lot.resterendeHoeveelheid} stuks
                </h3>

                <p className="fs-2 fw-bold text-danger mt-3">
                    € {lot.huidigePrijs?.toFixed(2)}
                </p>

                <p className="text-muted mb-1">
                    Max: € {lot.maximumPrijs?.toFixed(2)} · Min: € {lot.minimumPrijs?.toFixed(2)}
                </p>

                <p className="text-muted">
                    Daling: € {lot.dalingPerSeconde?.toFixed(2)} per seconde
                </p>
            </div>
        </div>
    );
}
