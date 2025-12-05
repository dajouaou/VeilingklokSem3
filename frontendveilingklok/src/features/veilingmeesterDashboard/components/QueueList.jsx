export default function QueueList({ queue }) {
    return (
        <div className="card">
            <h2>Queue</h2>

            {queue.map((g) => (
                <div key={g.aanvoerderId} className="queue-group">
                    <h3>{g.aanvoerderNaam}</h3>

                    {g.items.map((item) => (
                        <div key={item.veilingProductId} className="queue-item">
                            <img src={item.fotoUrl} alt="" />
                            <div>{item.productNaam}</div>
                            <span>#{item.volgorde}</span>
                        </div>
                    ))}
                </div>
            ))}
        </div>
    );
}
