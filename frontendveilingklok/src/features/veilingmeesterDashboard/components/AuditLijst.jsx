export default function AuditLijst({ audit }) {
    if (!audit || audit.length === 0)
        return <p className="text-muted">Er zijn nog geen logboekmeldingen.</p>;

    return (
        <div className="card shadow-sm p-3 mb-4">
            <h5 className="mb-3">Auditlog</h5>

            <ul className="list-group list-group-flush">
                {audit.map((a, i) => (
                    <li key={i} className="list-group-item">
                        <span className="text-muted">
                            {a.tijdstip
                                ? new Date(a.tijdstip).toLocaleTimeString()
                                : ""}
                            {" – "}
                        </span>
                        {a.gebeurtenis}
                    </li>
                ))}
            </ul>
        </div>
    );
}
