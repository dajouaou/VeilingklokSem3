export default function CurrentProductCard({ product }) {
    if (!product) {
        return <div className="card">Geen actief product</div>;
    }

    return (
        <div className="card">
            <h2>Actief product</h2>

            {product.product?.fotoUrl && (
                <img
                    src={product.product.fotoUrl}
                    alt={product.product.naam}
                    style={{ width: "150px", borderRadius: "8px" }}
                />
            )}

            <p><strong>Naam:</strong> {product.product.naam}</p>
            <p><strong>Aanvoerder:</strong> {product.aanvoerder.naam}</p>
            <p><strong>Huidige prijs:</strong> €{product.huidigePrijs}</p>
            <p><strong>Hoeveelheid:</strong> {product.hoeveelheid} stuks</p>
        </div>
    );
}