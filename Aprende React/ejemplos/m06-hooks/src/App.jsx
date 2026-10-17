import UseEffectDemo from "./components/UseEffectDemo";
import UseRefDemo from "./components/UseRefDemo";
import UseMemoDemo from "./components/UseMemoDemo";
import CustomHookDemo from "./components/CustomHookDemo";

export default function App() {
  return (
    <div className="app">
      <h1>Módulo 6 · Hooks</h1>
      <section>
        <h2>1. useEffect — Sincronizar con el exterior</h2>
        <UseEffectDemo />
      </section>
      <section>
        <h2>2. useRef — Referencias persistentes</h2>
        <UseRefDemo />
      </section>
      <section>
        <h2>3. useMemo — Memorizar cálculos</h2>
        <UseMemoDemo />
      </section>
      <section>
        <h2>4. Custom hook — useLocalStorage</h2>
        <CustomHookDemo />
      </section>
    </div>
  );
}
