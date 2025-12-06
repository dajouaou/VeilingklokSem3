import { useState, useContext } from "react";
import { loginApi } from "../api/authApi";
import { AuthContext } from "./AuthContext";
import { useNavigate, Link } from "react-router-dom";

export default function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();
    const { loginUser } = useContext(AuthContext);

    async function handleSubmit(e) {
        e.preventDefault();
        setError("");

        try {
            const { token, role } = await loginApi({ email, password });
            loginUser(token, role);

            alert("Succesvol ingelogd!");

            if (role === "Koper") navigate("/");
            if (role === "Aanvoerder") navigate("/aanvoerder");
            if (role === "VM") navigate("/veilingmeester");

        } catch (err) {
            setError(err.message);
        }
    }

    return (
        <div className="d-flex justify-content-center align-items-center min-vh-100 bg-light">
            <div className="col-md-6 col-lg-4">
                <div className="card shadow-sm border-0 rounded-4 p-4">

                    <h2 className="fw-bold mb-3 text-success text-center">Inloggen</h2>

                    {error && <div className="alert alert-danger">{error}</div>}

                    <form onSubmit={handleSubmit}>
                        <div className="mb-3">
                            <input
                                type="email"
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
                        <Link to="/register" className="text-success fw-semibold">
                            Registreer
                        </Link>
                    </p>

                </div>
            </div>
        </div>
    );
}
