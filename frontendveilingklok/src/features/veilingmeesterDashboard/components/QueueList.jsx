export default function QueueList({ queue }) {
    if (!queue || queue.length === 0)
        return <p className="text-muted">Geen producten in wachtrij</p>;

    return (
        <div className="card shadow-sm border-0">
            <div className="card-header bg-light fw-bold">Wachtrij</div>
            <ul className="list-group list-group-flush">
                {queue.map((item) => (
                    <li key={item.id} className="list-group-item d-flex justify-content-between">
                        <span>
                            {item.soort} ({item.hoeveelheid} stuks)
                        </span>
                        <span className="text-muted">Volgorde {item.volgorde}</span>
                    </li>
                ))}
            </ul>
        </div>
    );
}
