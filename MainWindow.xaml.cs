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

namespace AnimalsWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AnimalsHandler ahr = null;
        public MainWindow()
        {
            InitializeComponent();
            ahr = new AnimalsHandler();
        }

        private async void btn_ShowCat_Click(object sender, RoutedEventArgs e)
        {
            this.ClearForm();

            var tempCat = ahr.catsList[ahr.allCatsIndex];

            int tempAnimalID = Convert.ToInt32(tempCat.AnimalID);
            Animal tempAnimal = ahr.GetAnimal(tempAnimalID);

            lbl_Hobby.Content = "Likes catnip?";
            txtBox_Hobby.Text = tempCat.LikesCatnip.ToString();

            int tempCatBreedID = Convert.ToInt32(tempCat.CatBreed);
            var tempWorker = ahr.AsyncGetCatBreed(tempCatBreedID);

            int tempDomecileID = Convert.ToInt32(tempCat.DomecileID);
            var tempWorker1 = ahr.AsyncGetDomecile(tempDomecileID);

            var tempWorker2 = ahr.AsyncGetAnimalName(tempAnimal);
            var tempWorker3 = ahr.AsyncGetAnimalBirthday(tempAnimal);
            var tempWorker4 = ahr.AsyncGetAnimalWeight(tempAnimal);

            List<Task<string>> tempWorkerTasks = new List<Task<string>>();
            tempWorkerTasks.Add(tempWorker);
            tempWorkerTasks.Add(tempWorker1);
            tempWorkerTasks.Add(tempWorker2);
            tempWorkerTasks.Add(tempWorker3);
            tempWorkerTasks.Add(tempWorker4);

            while (tempWorkerTasks.Count > 0)
            {
                var tempResult = await Task.WhenAny(tempWorkerTasks);
                tempWorkerTasks.Remove(tempResult);
                try
                {
                    await tempResult;
                    if(tempResult == tempWorker)
                    {
                        txtBox_Breed.Text = tempResult.Result;
                    }
                    else if (tempResult == tempWorker1)
                    {
                        txtBox_Home.Text = tempResult.Result;
                    }
                    else if (tempResult == tempWorker2)
                    {
                        txtBox_Name.Text = tempResult.Result;

                    }
                    else if (tempResult == tempWorker3)
                    {
                        txtBox_Birthday.Text = tempResult.Result;
                    }
                    else if (tempResult == tempWorker4)
                    {
                        txtBox_Weight.Text = tempResult.Result;

                    }
                    else
                    {
                        throw new Exception();
                    };
                }
                catch(Exception exc)
                {
                    throw exc;
                }
            }

            ahr.UpdateCatsIndex();
        }

        private async void btn_ShowDog_Click(object sender, RoutedEventArgs e)
        {
            this.ClearForm();
            var tempDog = ahr.dogsList[ahr.allDogsIndex];
            lbl_Hobby.Content = "Plays fetch?";
            txtBox_Hobby.Text = tempDog.PlaysFetch.ToString();

            int tempDogBreedID = Convert.ToInt32(tempDog.DogBreedID);
            var tempWorker = ahr.AsyncGetDogBreed(tempDogBreedID);
            await tempWorker;
            txtBox_Breed.Text = tempWorker.Result;

            int tempDomecileID = Convert.ToInt32(tempDog.DomecileID);
            var tempWorker1 = ahr.AsyncGetDomecile(tempDomecileID);
            await tempWorker1;
            txtBox_Home.Text = tempWorker1.Result;

            int tempAnimalID = Convert.ToInt32(tempDog.AnimalID);
            var tempWorker2 = ahr.AsyncGetAnimal(tempAnimalID);
            await tempWorker2;
            Animal tempAnimal = tempWorker2.Result;

            txtBox_Name.Text = tempAnimal.Name.ToString();
            txtBox_Weight.Text = tempAnimal.Weight.ToString();
            txtBox_Birthday.Text = tempAnimal.Birthday.ToString();

            ahr.UpdateDogsIndex();
        }

        private void btn_AddNewCat_Click(object sender, RoutedEventArgs e)
        {
            lbl_Hobby.Content = "Likes catnip?";
            txtBox_Hobby.Text = "";
            txtBox_Breed.Text = "";
            txtBox_Home.Text = "";
            txtBox_Name.Text = "";
            txtBox_Weight.Text = "";
            txtBox_Birthday.Text = "";
        }

        private void btn_AddNewDog_Click(object sender, RoutedEventArgs e)
        {
            lbl_Hobby.Content = "Plays fetch?";
            txtBox_Hobby.Text = "";
            txtBox_Breed.Text = "";
            txtBox_Home.Text = "";
            txtBox_Name.Text = "";
            txtBox_Weight.Text = "";
            txtBox_Birthday.Text = "";

        }

        private void lbl_Update_Click(object sender, RoutedEventArgs e)
        {
            string tempAnimalType = "";
            bool wasAdded = false;
            switch (lbl_Hobby.Content)
            {
                case "Plays fetch?":
                    tempAnimalType = "Dog";
                    bool tempHobby = Convert.ToBoolean(txtBox_Hobby.Text);
                    //MessageBox.Show("Adding a "+tempAnimalType);
                    if (ConfirmAddition(tempAnimalType))
                    {
                        int tempDomecileID = ahr.GetKeyDomecile(txtBox_Home.Text);
                        int tempDogBreedID = ahr.GetKeyDogBreed(txtBox_Breed.Text);
                        int tempAnimalID = ahr.GetKeyAnimal(txtBox_Name.Text, txtBox_Weight.Text, txtBox_Birthday.Text);
                        wasAdded = ahr.AddCritter(tempAnimalType, tempAnimalID, 
                            tempDogBreedID, tempDomecileID, tempHobby);
                    }
                    break;
                case "Likes catnip?":
                    tempAnimalType = "Cat";
                    bool tempHobby2 = Convert.ToBoolean(txtBox_Hobby.Text);
                    //MessageBox.Show("Adding a "+tempAnimalType);
                    if (ConfirmAddition(tempAnimalType))
                    {
                        int tempDomecileID2 = ahr.GetKeyDomecile(txtBox_Home.Text);
                        int tempCatBreedID = ahr.GetKeyCatBreed(txtBox_Breed.Text);
                        int tempAnimalID = ahr.GetKeyAnimal(txtBox_Name.Text, txtBox_Weight.Text, txtBox_Birthday.Text);
                        wasAdded = ahr.AddCritter(tempAnimalType, tempAnimalID,
                            tempCatBreedID, tempDomecileID2, tempHobby2);
                    }
                    break;
            }
            if(wasAdded)
            {
                MessageBox.Show("Record added successfully.");
            }

            ClearForm();
        }
        private bool ConfirmAddition(string tempAnimalType)
        {
            string tempHobby = txtBox_Hobby.Text;
            string tempBreed = txtBox_Breed.Text;
            string tempHome = txtBox_Home.Text;
            string tempName = txtBox_Name.Text;
            string tempWeight = txtBox_Weight.Text;
            string tempBirthday = txtBox_Birthday.Text;

            string tempMessage = tempAnimalType + tempBirthday + tempBreed + tempHobby + tempHome + tempName + tempWeight;
            var result = MessageBox.Show(tempMessage, "Add to database?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void ClearForm()
        {
            lbl_Hobby.Content = "Hobby";
            txtBox_Hobby.Text = "";
            txtBox_Breed.Text = "";
            txtBox_Home.Text = "";
            txtBox_Name.Text = "";
            txtBox_Weight.Text = "";
            txtBox_Birthday.Text = "";
        }
    }
}
