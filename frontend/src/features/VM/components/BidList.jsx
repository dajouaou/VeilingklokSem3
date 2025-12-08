// src/features/VM/components/BidList.jsx
import { formatTime } from "../utils/formatters";

function formatAmount(amount) {
    if (amount === null || amount === undefined) return "—";
    if (typeof amount === "number") {
        return amount.toFixed(2);
    }
    if (typeof amount.toFixed === "function") {
        return amount.toFixed(2);
    }
    return String(amount);
}

export default function BidList({ bids }) {
    if (!bids || bids.length === 0) {
        return null;
    }

    return (
        <ul className="vm-bid-list">
            {bids.map((bid) => {
                const amount = formatAmount(bid.amount);
                const koperNaam = bid.koperNaam ?? "Onbekende koper";
                const tijd =
                    bid.placedAtUtc !== null && bid.placedAtUtc !== undefined
                        ? formatTime(bid.placedAtUtc)
                        : null;

                return (
                    <li key={bid.id} className="vm-bid-item">
                        <div className="vm-bid-main">€{amount}</div>
                        <div className="vm-bid-meta">
                            door {koperNaam}
                            {tijd && (
                                <>
                                    {" "}
                                    •{" "}
                                    <span className="vm-bid-time">{tijd}</span>
                                </>
                            )}
                        </div>
                    </li>
                );
            })}
        </ul>
    );
}
