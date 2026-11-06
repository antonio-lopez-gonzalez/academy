import test from "node:test";
import assert from "node:assert/strict";
import { getProfile, listProfiles, PROFILES } from "../src/profiles.mjs";

test("todos los perfiles tienen etapas y descripción", () => {
  for (const [id, profile] of Object.entries(PROFILES)) {
    assert.ok(profile.title, `${id} sin title`);
    assert.ok(profile.description, `${id} sin description`);
    assert.ok(profile.stages.length > 0, `${id} sin etapas`);
    for (const stage of profile.stages) {
      assert.ok(stage.id, `${id} con etapa sin id`);
      assert.ok(stage.title, `${id} con etapa sin title`);
    }
  }
});

test("listProfiles devuelve los 11 perfiles", () => {
  assert.equal(listProfiles().length, 11);
});

test("getProfile falla con un perfil desconocido", () => {
  assert.throws(() => getProfile("nope"), /perfil desconocido/);
});

test("classic incluye plan y test", () => {
  const ids = getProfile("classic").stages.map((stage) => stage.id);
  assert.deepEqual(ids, ["intent", "requirements", "design", "plan", "build", "test"]);
});
