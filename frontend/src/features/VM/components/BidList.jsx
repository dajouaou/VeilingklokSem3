// src/components/BidList.jsx

export default function BidList({ bids }) {
    if (!bids || bids.length === 0) {
        
        return null;
    }

    return (
        <ul className="vm-bid-list">
            {bids.map((bid) => (
                <li key={bid.id} className="vm-bid-item">
                    <div className="vm-bid-main">
                        €{bid.amount?.toFixed?.(2) ?? bid.amount}
                    </div>
                    <div className="vm-bid-meta">
                        door {bid.koperNaam ?? "Onbekende koper"}
                        {bid.placedAtUtc && (
                            <>
                                {" "}
                                •{" "}
                                <span className="vm-bid-time">
                  {new Date(bid.placedAtUtc).toLocaleTimeString("nl-NL")}
                </span>
                            </>
                        )}
                    </div>
                </li>
            ))}
        </ul>
    );
}
