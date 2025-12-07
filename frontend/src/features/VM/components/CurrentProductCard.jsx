// src/components/CurrentProductCard.jsx

export default function CurrentProductCard({ product }) {
    if (!product) {
     
        return null;
    }

    return (
        <div className="vm-current-product">
            
            <div className="vm-current-main">
                <div className="vm-current-name">
                    {product.productNaam ?? "Onbekend product"}
                </div>

                <div className="vm-current-meta">
                    <span className="vm-current-label">Aanvoerder:</span>
                    <span>{product.aanvoerder ?? "Onbekend"}</span>
                </div>

                <div className="vm-current-meta">
                    <span className="vm-current-label">Hoeveelheid:</span>
                    <span>{product.hoeveelheid} stuks</span>
                </div>

                <div className="vm-current-prices">
                    <div>
                        <span className="vm-current-label">Startprijs:</span>
                        <span>€{product.startPrijs?.toFixed?.(2) ?? product.startPrijs}</span>
                    </div>
                    <div className="vm-current-price-now">
                        <span className="vm-current-label">Huidige prijs:</span>
                        <span className="vm-current-price-value">
              €{product.huidigePrijs?.toFixed?.(2) ?? product.huidigePrijs}
            </span>
                    </div>
                </div>
            </div>
        </div>
    );
}
