export default function FormSummary({ data }) {
  const countryNames = {
    es: "España",
    mx: "México",
    ar: "Argentina",
    co: "Colombia",
    cl: "Chile",
    pe: "Perú",
  };

  return (
    <div className="form-summary">
      <h3>Resumen del registro</h3>
      <ul>
        <li><strong>Nombre:</strong> {data.name || "—"}</li>
        <li><strong>Email:</strong> {data.email || "—"}</li>
        <li><strong>País:</strong> {countryNames[data.country] || "—"}</li>
        <li><strong>Plan:</strong> {data.plan || "—"}</li>
        <li><strong>Términos:</strong> {data.acceptTerms ? "Aceptados" : "Pendientes"}</li>
      </ul>
    </div>
  );
}
