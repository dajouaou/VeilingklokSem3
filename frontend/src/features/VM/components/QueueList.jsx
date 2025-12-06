export default function QueueList({ queue }) {
    if (!queue || queue.length === 0) {
        return (
            <div className="card">
                <h2>Queue</h2>
                <p>Geen producten in queue</p>
            </div>
        );
    }

    return (
        <div className="card">
            <h2>Queue</h2>

            <ul>
                {queue.map(item => (
                    <li key={item.id}>
                        {item.productNaam} — door {item.aanvoerder} (volgorde {item.volgorde})
                    </li>
                ))}
            </ul>
        </div>
    );
}
