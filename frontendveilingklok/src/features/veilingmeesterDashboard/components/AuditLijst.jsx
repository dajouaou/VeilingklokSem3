export default function AuditLijst({ audit }) {
    if (!audit || audit.length === 0) {
        return <p className="vm-muted">Er zijn nog geen logboekmeldingen.</p>;
    }

    return (
        <section className="vm-panel">
            <header className="vm-panel-header">
                <h3>Auditlog</h3>
                <p className="vm-muted">Gebeurtenissen tijdens de veiling.</p>
            </header>

            <ul className="vm-list">
                {audit.map((a, i) => (
                    <li key={i} className="vm-list-item">
                        <span className="vm-time">
                            {a.tijdstip ? new Date(a.tijdstip).toLocaleTimeString() : ""}
                        </span>
                        <span className="vm-sep">-</span>
                        <span className="vm-text">{a.gebeurtenis}</span>
                    </li>
                ))}
            </ul>
        </section>
    );
}
