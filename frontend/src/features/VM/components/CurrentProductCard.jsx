// src/features/VM/components/CurrentProductCard.jsx
import { formatProductStatus } from "../utils/formatters";

export default function CurrentProductCard({ product }) {
    if (!product) {
        return null;
    }

    const statusLabel =
        product.status !== undefined && product.status !== null
            ? formatProductStatus(product.status)
            : null;

    const naam = product.productNaam ?? "Onbekend product";
    const aanvoerder = product.aanvoerder ?? "Onbekend";

    const hoeveelheid =
        product.hoeveelheid !== undefined && product.hoeveelheid !== null
            ? product.hoeveelheid
            : "—";

    const startPrijs =
        product.startPrijs !== undefined && product.startPrijs !== null
            ? product.startPrijs.toFixed
                ? product.startPrijs.toFixed(2)
                : product.startPrijs
            : "—";

    const huidigePrijs =
        product.huidigePrijs !== undefined && product.huidigePrijs !== null
            ? product.huidigePrijs.toFixed
                ? product.huidigePrijs.toFixed(2)
                : product.huidigePrijs
            : "—";

    return (
        <div className="vm-current-product">
            <div className="vm-current-main">
                <div className="vm-current-name">{naam}</div>

                {statusLabel && (
                    <div className="vm-current-meta">
                        <span className="vm-current-label">Status:</span>
                        <span>{statusLabel}</span>
                    </div>
                )}

                <div className="vm-current-meta">
                    <span className="vm-current-label">Aanvoerder:</span>
                    <span>{aanvoerder}</span>
                </div>

                <div className="vm-current-meta">
                    <span className="vm-current-label">Hoeveelheid:</span>
                    <span>
                        {hoeveelheid}
                        {hoeveelheid !== "—" ? " stuks" : ""}
                    </span>
                </div>

                <div className="vm-current-prices">
                    <div>
                        <span className="vm-current-label">Startprijs:</span>
                        <span>€{startPrijs}</span>
                    </div>
                    <div className="vm-current-price-now">
                        <span className="vm-current-label">Huidige prijs:</span>
                        <span className="vm-current-price-value">
                            €{huidigePrijs}
                        </span>
                    </div>
                </div>
            </div>
        </div>
    );
}
