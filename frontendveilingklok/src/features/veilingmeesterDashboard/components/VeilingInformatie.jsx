export default function VeilingInformatie({ gegevens }) {
    if (!gegevens)
        return <p className="text-muted">Geen informatie over de veiling beschikbaar.</p>;

    const statusParts = [];
    if (gegevens.isGestart) statusParts.push("Gestart");
    else statusParts.push("Niet gestart");
    if (gegevens.isPauze) statusParts.push("Pauze");
    if (gegevens.isAfgesloten) statusParts.push("Afgesloten");

    const statusText = statusParts.join(" · ");

    return (
        <div className="card shadow-sm p-3 mb-4">
            <h5 className="mb-3">Veilinginformatie</h5>

            <p>
                <strong>Veilingnummer:</strong> {gegevens.id}
            </p>

            <p>
                <strong>Status:</strong> {statusText}
            </p>

            {gegevens.huidigProduct && (
                <p>
                    <strong>Huidig product:</strong>{" "}
                    {gegevens.huidigProduct.soort} (
                    {gegevens.huidigProduct.hoeveelheid} stuks)
                </p>
            )}

            <p>
                <strong>Aantal in wachtrij:</strong>{" "}
                {Array.isArray(gegevens.wachtrij)
                    ? gegevens.wachtrij.length
                    : 0}
            </p>
        </div>
    );
}
