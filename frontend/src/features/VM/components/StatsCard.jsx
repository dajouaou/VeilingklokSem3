// src/features/VM/components/StatsCard.jsx
export default function StatsCard({
                                      totalBidsCurrentProduct,
                                      totaalBiedingenVeiling,
                                      productenInQueue,
                                      verkochteProducten,
                                      totaalProducten,
                                  }) {
    const bidsCurrent = totalBidsCurrentProduct ?? 0;
    const bidsTotal = totaalBiedingenVeiling ?? 0;
    const queueCount = productenInQueue ?? 0;
    const soldCount = verkochteProducten ?? 0;
    const totalProducts = totaalProducten ?? 0;

    return (
        <div className="vm-stats">
            <div className="vm-stats-row">
                <span className="vm-stats-label">Biedingen op huidig product</span>
                <span className="vm-stats-value">{bidsCurrent}</span>
            </div>

            <div className="vm-stats-row">
                <span className="vm-stats-label">Totaal aantal biedingen in deze veiling</span>
                <span className="vm-stats-value">{bidsTotal}</span>
            </div>

            <div className="vm-stats-row">
                <span className="vm-stats-label">Producten verkocht</span>
                <span className="vm-stats-value">{soldCount}</span>
            </div>

            <div className="vm-stats-row">
                <span className="vm-stats-label">Producten in queue</span>
                <span className="vm-stats-value">{queueCount}</span>
            </div>

            <div className="vm-stats-row">
                <span className="vm-stats-label">Totaal producten in veiling</span>
                <span className="vm-stats-value">{totalProducts}</span>
            </div>
        </div>
    );
}
