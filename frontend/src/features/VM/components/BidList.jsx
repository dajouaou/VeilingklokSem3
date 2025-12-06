export default function BidList({ bids }) {
    return (
        <div className="card">
            <h2>Biedingen</h2>

            {bids.length === 0 && <p>Geen biedingen geplaatst.</p>}

            <ul>
                {bids.map(bid => (
                    <li key={bid.id}>
                        €{bid.bedrag} — door {bid.koper?.gebruiker?.naam ?? "Onbekend"}
                    </li>
                ))}
            </ul>
        </div>
    );
}