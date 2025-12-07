export default function LiveKlok({ lot }) {
    if (!lot) return <p>Geen actief product</p>;

    const { soort, hoeveelheid, huidigePrijs, startPrijs } = lot;

    return (
        <div className="card shadow-sm border-0 mb-4">
            <div className="card-body">
                <h3 className="h5">
                    {soort} – {hoeveelheid} stuks
                </h3>

                <p className="fs-2 fw-bold text-danger mt-3">
                    € {huidigePrijs?.toFixed(2)}
                </p>

                <p className="text-muted">
                    Startprijs: € {startPrijs?.toFixed(2)}
                </p>
            </div>
        </div>
    );
}
