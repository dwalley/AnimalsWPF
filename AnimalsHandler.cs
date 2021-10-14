using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

namespace AnimalsWPF
{
    class AnimalsHandler
    {
        private static bool _haveDBContext = false;

        public int allCatsIndex = 0;
        public int allDogsIndex = 0;

        public AnimalsDBEntities ctx = null;
        public List<Cat> allCatsList = null;
        public List<Dog> allDogsList = null;
        public List<Cat> catsList = null;
        public List<CatBreed> catBreedsList = null;
        public List<Animal> animalsList = null;
        public List<Dog> dogsList = null;
        public List<DogBreed> dogBreedsList = null;
        public List<Domecile> domecilesList = null;
        public AnimalsHandler()
        {
            //MessageBox.Show("In constructor");
            if(!_haveDBContext)
            {
                try
                {
                    ctx = new AnimalsDBEntities();
                    catsList = ctx.Cats.ToList();
                    catBreedsList = ctx.CatBreeds.ToList();
                    animalsList = ctx.Animals.ToList();
                    dogsList = ctx.Dogs.ToList();
                    dogBreedsList = ctx.DogBreeds.ToList();
                    domecilesList = ctx.Domeciles.ToList();
                    allCatsList = ctx.Cats.Include("CatBreed1").Include("Animal").Include("Domecile").ToList();
                    allDogsList = ctx.Dogs.Include("DogBreed").Include("Animal").Include("Domecile").ToList();
                    _haveDBContext = true;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                MessageBox.Show("already have DB context");
            }
        }
        public void UpdateCatsIndex()
        {
            if (this.allCatsIndex >= this.catsList.Count() - 1)
            {
                this.allCatsIndex = 0;
            }
            else
            {
                this.allCatsIndex += 1;
            }
        }
        public void UpdateDogsIndex()
        {
            if (this.allDogsIndex >= this.dogsList.Count() - 1)
            {
                this.allDogsIndex = 0;
            }
            else
            {
                this.allDogsIndex += 1;
            }
        }
        public int GetKeyDomecile(string tempDomecileName)
        {
            for (int i=0; i < domecilesList.Count(); ++i)
                if(domecilesList[i].DomecileName == tempDomecileName)
                {
                    return domecilesList[i].DomecileID;
                }
            //logic to update Domeciles table with the new name and return the new key
            Domecile tempRecord = new Domecile();
            tempRecord.DomecileName = tempDomecileName;
            ctx.Domeciles.Add(tempRecord);
            ctx.SaveChanges();
            this.domecilesList.Add(tempRecord);
            //MessageBox.Show(tempRecord.DomecileID.ToString());
            return tempRecord.DomecileID;
        }
        public int GetKeyDogBreed(string tempDogBreedName)
        {
            for (int i = 0; i < dogBreedsList.Count(); ++i)
                if (dogBreedsList[i].DogBreedName == tempDogBreedName)
                {
                    return dogBreedsList[i].DogBreedID;
                }
            //logic to update dog breeds table with the new name and return the new key
            DogBreed tempRecord = new DogBreed();
            tempRecord.DogBreedName = tempDogBreedName;
            ctx.DogBreeds.Add(tempRecord);
            ctx.SaveChanges();
            this.dogBreedsList.Add(tempRecord);
            //MessageBox.Show(tempRecord.DomecileID.ToString());
            return tempRecord.DogBreedID;
        }
        public int GetKeyCatBreed(string tempCatBreedName)
        {
            for (int i = 0; i < catBreedsList.Count(); ++i)
                if (catBreedsList[i].CatBreedName == tempCatBreedName)
                {
                    return catBreedsList[i].CatBreedID;
                }
            //logic to update catbreeds table with the new name and return the new key
            CatBreed tempRecord = new CatBreed();
            tempRecord.CatBreedName = tempCatBreedName;
            ctx.CatBreeds.Add(tempRecord);
            ctx.SaveChanges();
            this.catBreedsList.Add(tempRecord);
            //MessageBox.Show(tempRecord.DomecileID.ToString());
            return tempRecord.CatBreedID;
        }
        public int GetKeyAnimal(string tempName, string sTempWeight, string sTempBirthday)
        {
            for(int i = 0; i < animalsList.Count(); ++i)
            {
                if (animalsList[i].Name == tempName)
                {
                    MessageBox.Show(tempName + " is already known to us.");
                    return animalsList[i].AnimalID;
                }
            }

            // Convert sTempWeight to a double
            double tempWeight = Convert.ToDouble(sTempWeight);

            // Convert sTempBirthday to a DateTime
            DateTime tempBirthday = Convert.ToDateTime(sTempBirthday);

            //MessageBox.Show("Adding Animal" + tempName + tempWeight.ToString() + tempBirthday.ToString());

            //logic to update animals table with the new name and return the new key
            Animal tempRecord = new Animal();
            tempRecord.Name = tempName;
            tempRecord.Weight = tempWeight;
            tempRecord.Birthday = tempBirthday;
            ctx.Animals.Add(tempRecord);
            ctx.SaveChanges();
            this.animalsList.Add(tempRecord);
            return tempRecord.AnimalID;
        }
        public bool AddCritter(string tempAnimalType, int tempAnimalID, 
            int tempBreedID, int tempDocecileID, bool tempHobbby)
        {
            bool isNewCritter = true;

            switch (tempAnimalType)
            {
                case "Dog":
                    foreach (Dog tempDog in allDogsList)
                    {
                        if((tempDog.AnimalID == tempAnimalID) &&
                            (tempDog.DomecileID == tempDocecileID))
                        {
                            // Critter of type dog living at domecile is already in the database, do nothing
                            isNewCritter = false;
                            return isNewCritter;
                        }
                    }
                    // This is a new to us dog, add to database.
                    Dog addMe = new Dog();
                    addMe.AnimalID = tempAnimalID;
                    addMe.DogBreedID = tempBreedID;
                    addMe.DomecileID = tempDocecileID;
                    addMe.PlaysFetch = tempHobbby;
                    ctx.Dogs.Add(addMe);
                    ctx.SaveChanges();
                    this.dogsList.Add(addMe);
                    isNewCritter = true;
                    break;
                case "Cat":
                    foreach (Cat tempCat in allCatsList)
                    {
                        if ((tempCat.AnimalID == tempAnimalID) &&
                            (tempCat.DomecileID == tempDocecileID))
                        {
                            // Critter of type cat living at domecile is already in the database, do nothing
                            isNewCritter = false;
                            return isNewCritter;
                        }
                    }
                    // This is a new to us cat, add to database.
                    Cat addMe2 = new Cat();
                    addMe2.AnimalID = tempAnimalID;
                    addMe2.CatBreed = tempBreedID;
                    addMe2.DomecileID = tempDocecileID;
                    addMe2.LikesCatnip = tempHobbby;
                    ctx.Cats.Add(addMe2);
                    ctx.SaveChanges();
                    isNewCritter = true;
                    this.catsList.Add(addMe2);
                    break;
                default:
                    throw new InvalidOperationException();

            }

            return isNewCritter;
        }
        public string GetCatBreed(int tempBreedKey)
        {
            for (int i=0; i < catBreedsList.Count(); ++i)
            {
                if (tempBreedKey == this.catBreedsList[i].CatBreedID)
                {
                    return this.catBreedsList[i].CatBreedName;
                }
            }
            return "";
        }
        public Task<string> AsyncGetCatBreed(int tempBreedKey)
        {
            return Task.Run<string>(() =>
            {
                randomWait();
                return this.GetCatBreed(tempBreedKey);
            });
        }
        public string GetDogBreed(int tempBreedKey)
        {
            for (int i = 0; i < dogBreedsList.Count(); ++i)
            {
                if (tempBreedKey == this.dogBreedsList[i].DogBreedID)
                {
                    return this.dogBreedsList[i].DogBreedName;
                }
            }
            return "";
        }
        public Task<string> AsyncGetDogBreed(int tempBreedKey)
        {
            return Task.Run<string>(() =>
            {
                randomWait();
                return this.GetDogBreed(tempBreedKey);
            });
        }
        public string GetDomecile(int tempDomecileKey)
        {
            for (int i = 0; i < domecilesList.Count(); ++i)
            {
                if (tempDomecileKey == this.domecilesList[i].DomecileID)
                {
                    return this.domecilesList[i].DomecileName;
                }
            }
            return "";
        }
        public Task<string> AsyncGetDomecile(int tempDomecileKey)
        {
            return Task.Run<string>(() =>
            {
                randomWait();
                return this.GetDomecile(tempDomecileKey);
            });
        }
        public Animal GetAnimal(int tempAnimalKey)
        {
            for (int i = 0; i < animalsList.Count(); ++i)
            {
                if (tempAnimalKey == this.animalsList[i].AnimalID)
                {
                    return this.animalsList[i];
                }
            }
            return null;
        }
        public Task<Animal> AsyncGetAnimal(int tempAnimalKey)
        {
            return Task.Run<Animal>(() =>
            {
                randomWait();
                return this.GetAnimal(tempAnimalKey);
            });
        }
        public void randomWait()
        {
            // Wait randomly 2-5 seconds
            Random r = new Random(Guid.NewGuid().GetHashCode());
            int sleepTime = r.Next(1000, 4000);
            //MessageBox.Show(sleepTime.ToString());
            Thread.Sleep(sleepTime);
        }
        public string GetAnimalName(Animal tempAnimal)
        {            
            return tempAnimal.Name;
        }
        public Task<string> AsyncGetAnimalName(Animal tempAnimal)
        {
            return Task.Run<string>(() =>
            {
                randomWait();
                return this.GetAnimalName(tempAnimal);
            });
        }
        public string GetAnimalWeight(Animal tempAnimal)
        {
            return tempAnimal.Weight.ToString();
        }
        public Task<string> AsyncGetAnimalWeight(Animal tempAnimal)
        {
            return Task.Run<string>(() =>
            {
                randomWait();
                return this.GetAnimalWeight(tempAnimal);
            });
        }
        public string GetAnimalBirthday(Animal tempAnimal)
        {
            return tempAnimal.Birthday.ToString();
        }
        public Task<string> AsyncGetAnimalBirthday(Animal tempAnimal)
        {
            return Task.Run<string>(() =>
            {
                randomWait();
                return this.GetAnimalBirthday(tempAnimal);
            });
        }
    }
}
