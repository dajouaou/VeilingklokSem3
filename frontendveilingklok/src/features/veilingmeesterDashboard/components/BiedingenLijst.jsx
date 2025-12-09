export default function BiedingenLijst({ biedingen }) {
    if (!biedingen || biedingen.length === 0)
        return <p className="text-muted">Er zijn nog geen biedingen geplaatst.</p>;

    return (
        <div className="card shadow-sm p-3 mb-4">
            <h5 className="mb-3">Biedingen</h5>

            <ul className="list-group list-group-flush">
                {biedingen.map((b, i) => (
                    <li key={b.id || i} className="list-group-item d-flex justify-content-between">
                        <span>
                            € {b.prijs?.toFixed(2)}
                            {b.koperNaam && (
                                <span className="text-muted ms-2">({b.koperNaam})</span>
                            )}
                        </span>

                        <span className="text-muted">
                            {b.tijdstip
                                ? new Date(b.tijdstip).toLocaleTimeString()
                                : ""}
                        </span>
                    </li>
                ))}
            </ul>
        </div>
    );
}
