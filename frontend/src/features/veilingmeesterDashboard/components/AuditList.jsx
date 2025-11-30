export default function AuditList({ audit }) {
    return (
        <div className="card">
            <h2>Audit</h2>
            {audit.map((a) => (
                <div key={a.id} className="audit-item">
                    <strong>{a.action}</strong> – {a.details}
                </div>
            ))}
        </div>
    );
}

