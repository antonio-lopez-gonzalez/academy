import ControlledInputDemo from "./components/ControlledInputDemo";
import CheckboxRadioSelectDemo from "./components/CheckboxRadioSelectDemo";
import ValidationDemo from "./components/ValidationDemo";

export default function App() {
  return (
    <div className="app">
      <h1>Módulo 5 · Formularios</h1>
      <section>
        <h2>1. Input controlado</h2>
        <ControlledInputDemo />
      </section>
      <section>
        <h2>2. Checkbox, radio y select</h2>
        <CheckboxRadioSelectDemo />
      </section>
      <section>
        <h2>3. Validación en tiempo real</h2>
        <ValidationDemo />
      </section>
    </div>
  );
}
