#include <iostream>
#include <fstream>
#include <sstream>
#include <string>
#include <vector>
#include <iomanip>
#include <ctime>
#include <chrono>
//#include <gmpxx.h> // Pour manipuler des grands entiers avec GMP

// Fonction pour lire un fichier
std::string ReadFile(const std::string& filename) {
  std::ifstream file(filename);
  if (!file.is_open()) {
    return "2"; // Valeur par défaut
  }
  std::string line;
  std::getline(file, line);
  return line;
}

// Fonction pour écrire dans un fichier
static void WriteToFile(const std::string& filename, const std::string& message, bool append = false) {
  std::ofstream file(filename, append ? std::ios::app : std::ios::out);
  if (file.is_open()) {
    file << message << std::endl;
  }
}

// Fonction pour ajouter la date et l'heure au nom d'un fichier
//static std::string AddTimetoFilenameOLD(const std::string& filename) {
//  auto now = std::time(nullptr);
//  char buffer[80];
//  std::strftime(buffer, sizeof(buffer), "%Y-%m-%d_%H-%M-%S", std::localtime(&now));
//  return filename + "-" + buffer + ".txt";
//}

static std::string AddTimetoFilename(const std::string& filename) {
  auto now = std::time(nullptr); // Obtenir l'heure actuelle
  std::tm timeStruct = {};       // Structure pour stocker les informations de temps

  // Utiliser localtime_s pour remplir la structure timeStruct
  if (localtime_s(&timeStruct, &now) == 0) { // localtime_s retourne 0 en cas de succès
    char buffer[80]; // Taille suffisante pour le format
    std::strftime(buffer, sizeof(buffer), "%Y-%m-%d_%H-%M-%S", &timeStruct);
    return filename + "-" + buffer + ".txt";
  }
  else {
    throw std::runtime_error("Erreur lors de la conversion de l'heure locale.");
  }
}

// Fonction pour vérifier si un nombre est premier
//bool IsPrime(const mpz_class& number) {
  //return mpz_probab_prime_p(number.get_mpz_t(), 25) > 0; // Test probable de primalité
//}

// Fonction pour formater une durée
std::string FormatElapsedTime(std::chrono::seconds duration) {
  int hours = duration.count() / 3600;
  int minutes = (duration.count() % 3600) / 60;
  int seconds = duration.count() % 60;
  std::ostringstream oss;
  if (hours > 0) {
    oss << hours << " heure" << (hours > 1 ? "s " : " ");
  }
  if (minutes > 0) {
    oss << minutes << " minute" << (minutes > 1 ? "s " : " ");
  }
  oss << seconds << " seconde" << (seconds > 1 ? "s" : "");
  return oss.str();
}

int main() {
  std::cout << "Recherche de grands nombres premiers" << std::endl;

  auto now = std::time(nullptr);
  char buffer[80];
  //std::strftime(buffer, sizeof(buffer), "%Y-%m-%d_%H-%M-%S", std::localtime(&now));
  std::cout << "Début de la recherche : " << buffer << std::endl;

  // Lecture du dernier nombre traité
  std::string lastNumberComputed = ReadFile("lastNumber.txt");
  //mpz_class startNumber(lastNumberComputed);
  /*if (startNumber % 2 == 0) {
    startNumber++;
  }*/

  int increment = 1000; // Nombre à tester
  //mpz_class endNumber = startNumber + increment;

  //std::vector<mpz_class> primes;
  auto start_time = std::chrono::steady_clock::now();

  /*for (mpz_class i = startNumber; i < endNumber; i += 2) {
    if (IsPrime(i)) {
      std::cout << i << " est premier" << std::endl;
      primes.push_back(i);
    }
  }*/

  auto end_time = std::chrono::steady_clock::now();
  auto duration = std::chrono::duration_cast<std::chrono::seconds>(end_time - start_time);

  // Résumé
  std::cout << "Temps écoulé : " << FormatElapsedTime(duration) << std::endl;
  //std::cout << primes.size() << " nombre(s) premier(s) trouvé(s)" << std::endl;

  // Écriture des résultats
  //WriteToFile("lastNumber.txt", endNumber.get_str());
  WriteToFile(AddTimetoFilename("BigIntegerPrimes"), "Nombres premiers trouvés");
  return 0;
}
