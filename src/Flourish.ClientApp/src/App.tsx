import { useState, useEffect } from "react";
import "./App.css";
import axios from "axios";
import { useConfiguration, useFeatureFlag } from "./hooks/useFeatureFlag";

interface IWeatherForecast {
  dateFormatted: string;
  temperatureC?: number;
  temperatureF: number;
  summary: string;
}

const defaultWeatherForecasts: IWeatherForecast[] = [];

const App = () => {
  const { config: configMessage } = useConfiguration(
    "TestApp:Settings:Message"
  );
  const { config: configBackgroundColor } = useConfiguration(
    "TestApp:Settings:BackgroundColor"
  );
  const { enabled: showMeTheMoney } = useFeatureFlag("ShowMeTheMoney");

  const showConfigMessage = configMessage.toString().trim().length;

  const [weatherForecasts, setWeatherForecasts]: [
    IWeatherForecast[],
    (weatherForecasts: IWeatherForecast[]) => void
  ] = useState(defaultWeatherForecasts);
  const [loading, setLoading]: [boolean, (loading: boolean) => void] =
    useState<boolean>(true);
  const [error, setError]: [string, (error: string) => void] = useState("");

  useEffect(() => {
    if (showMeTheMoney) {
      axios
        .get<IWeatherForecast[]>("api/WeatherForecast", {
          headers: {
            "Content-Type": "application/json",
          },
          timeout: 1500,
        })
        .then((response) => {
          setWeatherForecasts(response.data);
          setLoading(false);
        })
        .catch((ex) => {
          const error = axios.isCancel(ex)
            ? "Request Cancelled"
            : ex.code === "ECONNABORTED"
            ? "A timeout has occurred"
            : ex.response.status === 404
            ? "Resource not found"
            : "The API is not available";
          setError(error);
          setLoading(false);
        });
    }
  }, [showMeTheMoney]);

  return (
    <div className="App">
      <header
        className="App-header"
        style={{ backgroundColor: configBackgroundColor }}
      >
        <h3>Flourish React</h3>
        {loading && <b>Loading.</b>}
        {showConfigMessage && <div>{configMessage}</div>}
        <br />
        {!error && showMeTheMoney && (
          <table className="table">
            <thead>
              <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
              </tr>
            </thead>
            <tbody>
              {weatherForecasts.map((forecast, index) => (
                <tr key={index}>
                  <td>{forecast.dateFormatted}</td>
                  <td>{forecast.temperatureC}</td>
                  <td>{forecast.temperatureF}</td>
                  <td>{forecast.summary}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
        {error && <b>{error}</b>}
      </header>
    </div>
  );
};

export default App;
