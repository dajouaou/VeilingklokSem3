export default function BidList({ bids }) {
    if (!bids || bids.length === 0) {
        return (
            <div className="card">
                <h2>Biedingen</h2>
                <p>Geen biedingen geplaatst.</p>
            </div>
        );
    }

    return (
        <div className="card">
            <h2>Biedingen</h2>

            <ul>
                {bids.map(bid => (
                    <li key={bid.id}>
                        €{bid.amount} — door {bid.koperNaam}
                    </li>
                ))}
            </ul>
        </div>
    );
}
