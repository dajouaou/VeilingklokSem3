import { useEffect, useState } from "react";
import Navbar from "./../shared/components/Navbar";
import Footer from "./../shared/components/Footer";
import { useContext } from "react";
import { AuthContext } from "../features/auth/AuthContext";


const API = "https://localhost:5174";

export default function ActueelBod() {

    const [product, setProduct] = useState(null);
    const [prijs, setPrijs] = useState(null);
    const { token, role } = useContext(AuthContext);


    async function loadData() {
        try {
            // haal huidige veiling
            const res = await fetch(`${API}/api/veiling-public/actueel`);
            if (!res.ok) return;

            const data = await res.json();

            // als niks actief is -> toon geen fout
            if (!data?.huidigProduct) return;

            setProduct(data.huidigProduct);

            // biedingen
            const bodRes = await fetch(`${API}/api/veiling-public/actueel/biedingen`);
            const bodList = bodRes.ok ? await bodRes.json() : [];

            if (bodList.length > 0)
                setPrijs(bodList[bodList.length - 1].prijs);
            else
                setPrijs(data.huidigProduct.startPrijs);

        } catch (err) {
            console.log("fout public veiling:", err);
        }
    }

    useEffect(() => {
        loadData();
        const t = setInterval(loadData, 3000);
        return () => clearInterval(t);
    }, []);

    async function neemDezePrijs() {
        if (!prijs || !token) return;

        await fetch(`https://localhost:56418/api/bod`, {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                prijs
            })
        });
    }


    if (!product)
        return (
            <>
                <Navbar />
                <div className="container pt-5">Laden...</div>
                <Footer />
            </>
        );

    return (
        <>
            <Navbar />

            <div className="container py-5">
                <h2 className="mb-4 fw-bold">Actueel bod</h2>

                <div className="row">
                    <div className="col-md-6">
                        <img
                            src={product.fotoUrl}
                            alt=""
                            className="img-fluid rounded"
                        />
                    </div>

                    <div className="col-md-6">
                        <span className="badge bg-success">Actief</span>
                        <h4 className="mt-2">{product.soort}</h4>
                        <small>Hoeveelheid: {product.hoeveelheid}</small>

                        <h5 className="mt-4">
                            Laatste prijs: € {prijs?.toFixed(2)}
                        </h5>

                        {role === "Koper" && (
                            <button
                                className="btn btn-dark mt-3"
                                onClick={neemDezePrijs}
                            >
                                Neem deze prijs
                            </button>
                        )}
                    </div>
                </div>
            </div>

            <Footer />
        </>
    );
}
