import { useState } from "react";
import { loginApi } from "./api/authApi.js";
import { Link } from "react-router-dom";

export default function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    async function handleSubmit(e) {
        e.preventDefault();

        try {
            const result = await loginApi(email, password);
            alert("Inloggen succesvol! Token: " + result.token);
        } catch (err) {
            setError("Login mislukt: " + err.message);
        }
    }

    return (
        <div className="min-h-screen bg-gray-100 flex flex-col">
            <header className="bg-white shadow sticky top-0 p-4 flex justify-between">
                <h1 className="text-green-600 font-bold">Veilingklok</h1>
                <Link to="/register" className="text-green-600 font-semibold">
                    Registreren
                </Link>
            </header>

            <div className="flex justify-center items-center flex-1">
                <div className="bg-white shadow-lg p-6 rounded-xl w-full max-w-md">
                    <h2 className="text-green-600 text-center text-3xl font-bold mb-2">
                        Inloggen
                    </h2>

                    {error && (
                        <p className="text-red-500 text-sm mb-3 text-center">
                            {error}
                        </p>
                    )}

                    <form onSubmit={handleSubmit}>
                        <input
                            type="text"
                            placeholder="E-mailadres"
                            className="w-full border p-2 rounded mb-3"
                            onChange={(e) => setEmail(e.target.value)}
                        />

                        <input
                            type="password"
                            placeholder="Wachtwoord"
                            className="w-full border p-2 rounded mb-3"
                            onChange={(e) => setPassword(e.target.value)}
                        />

                        <button className="bg-green-600 text-white w-full py-2 rounded mt-2">
                            Log in
                        </button>
                    </form>

                    <p className="mt-4 text-center text-sm">
                        Nog geen account?{" "}
                        <Link to="/register" className="text-green-600 font-medium">
                            Registreer hier
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
}
