import { useState, useContext } from "react";
import { loginApi } from "./api/authApi";
import { AuthContext } from "./AuthContext";
import { useNavigate } from "react-router-dom";

export default function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const { login, role } = useContext(AuthContext);
    const navigate = useNavigate();

    async function handleSubmit(e) {
        e.preventDefault();
        setError("");

        try {
            const { token } = await loginApi({ email, password });

            // ?? JWT opslaan (rol komt nog uit backend later)
            login(token, "Koper"); // placeholder ? jij krijgt straks echte rol uit backend

            // Redirect op basis van rol
            navigate("/koper");
        } catch (err) {
            setError(err.message);
        }
    }

    return (
        <div className="bg-light">
            <div className="container py-5">
                <div className="row justify-content-center">
                    <div className="col-md-6 col-lg-4">
                        <div className="card shadow-sm border-0 rounded-4 p-4">

                            <h2 className="fw-bold mb-3 text-success text-center">
                                Inloggen
                            </h2>

                            {error && (
                                <div className="alert alert-danger">{error}</div>
                            )}

                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <input
                                        type="text"
                                        className="form-control"
                                        placeholder="E-mailadres"
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                        required
                                    />
                                </div>

                                <div className="mb-3">
                                    <input
                                        type="password"
                                        className="form-control"
                                        placeholder="Wachtwoord"
                                        value={password}
                                        onChange={(e) => setPassword(e.target.value)}
                                        required
                                    />
                                </div>

                                <button className="btn btn-success w-100 rounded-pill">
                                    Log in
                                </button>
                            </form>

                            <p className="text-center mt-4">
                                Nog geen account?{" "}
                                <a href="/register" className="text-success fw-semibold">
                                    Registreer
                                </a>
                            </p>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
