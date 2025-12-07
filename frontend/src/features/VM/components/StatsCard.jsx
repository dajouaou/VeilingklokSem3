// src/components/StatsCard.jsx

export default function StatsCard({ totalBids }) {
    return (
        <div className="vm-stats">
            <div className="vm-stats-row">
                <span className="vm-stats-label">Totaal aantal biedingen</span>
                <span className="vm-stats-value">{totalBids}</span>
            </div>
            
        </div>
    );
}
