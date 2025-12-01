import { useState } from "react";
import { mockRegister } from "./authMockService";
import { Link } from "react-router-dom";

export default function Register() {
    const [data, setData] = useState({
        username: "",
        email: "",
        password: "",
        role: "Koper" // default
    });

    const [message, setMessage] = useState("");

    function handleChange(e) {
        setData({ ...data, [e.target.name]: e.target.value });
    }

    async function handleSubmit(e) {
        e.preventDefault();
        const result = await mockRegister(data);
        setMessage(result.message);
    }

    return (
        <div className="min-h-screen bg-gray-100 flex flex-col">
            <header className="bg-white shadow p-4 flex justify-between">
                <h1 className="text-green-600 text-xl font-bold">Veilingklok</h1>
                <Link to="/login" className="text-green-600">
                    Inloggen
                </Link>
            </header>

            <div className="flex justify-center items-center flex-1">
                <div className="bg-white shadow-lg p-6 rounded-xl w-full max-w-md">
                    <h2 className="text-green-600 text-center text-3xl font-bold mb-2">
                        Registreren
                    </h2>

                    {message && <p className="text-center text-blue-600 mb-3">{message}</p>}

                    <form onSubmit={handleSubmit}>
                        <input
                            name="username"
                            placeholder="Gebruikersnaam"
                            className="w-full border p-2 rounded mb-3"
                            onChange={handleChange}
                        />
                        <input
                            name="email"
                            type="email"
                            placeholder="E-mailadres"
                            className="w-full border p-2 rounded mb-3"
                            onChange={handleChange}
                        />
                        <input
                            name="password"
                            type="password"
                            placeholder="Wachtwoord"
                            className="w-full border p-2 rounded mb-3"
                            onChange={handleChange}
                        />

                        {/* Role keuze */}
                        <select
                            name="role"
                            className="border p-2 w-full rounded mb-3"
                            onChange={handleChange}
                        >
                            <option value="Koper">Koper</option>
                            <option value="Aanvoerder">Aanvoerder</option>
                        </select>

                        <button className="bg-green-600 text-white w-full py-2 rounded mt-2">
                            Account aanmaken
                        </button>
                    </form>

                    <p className="mt-4 text-center text-sm">
                        Al een account?{" "}
                        <Link to="/login" className="text-green-600 font-medium">
                            Log in
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
}
