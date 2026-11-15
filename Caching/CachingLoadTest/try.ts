import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
  // iterations: 10,
  vus: 10,
  duration: "30s"
};

export default function () {
  const res = http.get("http://localhost:5063/api/customers");

  check(res, { "status was 200": (r) => r.status == 200 });

  sleep(1);
}
