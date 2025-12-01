export default function BidList({ bids }) {
    return (
        <div className="card">
            <h2>Biedingen</h2>
            {bids.map((b) => (
                <div key={b.id} className="bid-item">
                    <strong>€{b.amount}</strong> door {b.koperNaam || "Veilingmeester"}
                </div>
            ))}
        </div>
    );
}

