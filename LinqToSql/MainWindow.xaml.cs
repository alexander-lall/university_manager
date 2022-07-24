using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace LinqToSql
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LinqToSqlDataClassesDataContext SqlDataContext { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["LinqToSql.Properties.Settings.universityDBConnectionString"].ConnectionString;
            SqlDataContext = new LinqToSqlDataClassesDataContext(connectionString);

            UpdateTonnie();
        }

        public void InsertUniversities()
        {
            SqlDataContext.ExecuteCommand("DELETE FROM University");

            University yale = new University();
            yale.name = "Yale";
            SqlDataContext.Universities.InsertOnSubmit(yale);

            University osna = new University();
            osna.name = "FH Osnabrück";
            SqlDataContext.Universities.InsertOnSubmit(osna);

            SqlDataContext.SubmitChanges();

            MainDataGrid.ItemsSource = SqlDataContext.Universities;
        }

        public void InsertStudents()
        {
            SqlDataContext.ExecuteCommand("DELETE FROM Student");
            
            University yale = SqlDataContext.Universities.First(uni => uni.name.Equals("Yale"));
            University osna = SqlDataContext.Universities.First(uni => uni.name.Equals("FH Osnabrück"));

            List<Student> students = new List<Student>();
            students.Add(new Student() { name = "Carla", gender = "female", University = yale });
            students.Add(new Student() { name = "Leyla", gender = "female", University = yale });
            students.Add(new Student() { name = "Wu", gender = "trans-gender", University = osna });
            students.Add(new Student() { name = "Tonnie", gender = "male", University = osna });

            SqlDataContext.Students.InsertAllOnSubmit(students);
            SqlDataContext.SubmitChanges();

            MainDataGrid.ItemsSource = SqlDataContext.Students;
        }

        public void InsertLectures()
        {
            Lecture math = new Lecture();
            math.name = "Math";
            SqlDataContext.Lectures.InsertOnSubmit(math);

            Lecture engineering = new Lecture();
            engineering.name = "Engineering";
            SqlDataContext.Lectures.InsertOnSubmit(engineering);

            Lecture cs = new Lecture();
            cs.name = "Computer Science";
            SqlDataContext.Lectures.InsertOnSubmit(cs);

            SqlDataContext.SubmitChanges();

            MainDataGrid.ItemsSource = SqlDataContext.Lectures;
        }

        public void InsertStudentLecturesAssociation()
        {
            Student carla = SqlDataContext.Students.First(student => student.name.Equals("Carla"));
            Student leyla = SqlDataContext.Students.First(student => student.name.Equals("Leyla"));
            Student wu = SqlDataContext.Students.First(student => student.name.Equals("Wu"));
            Student tonnie = SqlDataContext.Students.First(student => student.name.Equals("Tonnie"));

            Lecture math = SqlDataContext.Lectures.First(lecture => lecture.name.Equals("Math"));
            Lecture engineering = SqlDataContext.Lectures.First(lecture => lecture.name.Equals("Engineering"));
            Lecture cs = SqlDataContext.Lectures.First(lecture => lecture.name.Equals("Computer Science"));

            List<Student_Lecture> ass = new List<Student_Lecture>();
            ass.Add(new Student_Lecture() { Lecture = math, Student = carla });
            ass.Add(new Student_Lecture() { Lecture = engineering, Student = carla });
            ass.Add(new Student_Lecture() { Lecture = cs, Student = carla });
            ass.Add(new Student_Lecture() { Lecture = math, Student = leyla });
            ass.Add(new Student_Lecture() { Lecture = math, Student = wu });
            ass.Add(new Student_Lecture() { Lecture = math, Student = tonnie });

            SqlDataContext.Student_Lectures.InsertAllOnSubmit(ass);
            SqlDataContext.SubmitChanges();

            MainDataGrid.ItemsSource = SqlDataContext.Student_Lectures;
        }

        public void GetUniverityOfTonnie()
        {
            Student tonnie = SqlDataContext.Students.First(student => student.name.Equals("Tonnie"));

            University tonniesUniversity = tonnie.University;

            List<University> universities = new List<University>() { tonniesUniversity };

            MainDataGrid.ItemsSource = universities;
        }

        public void GetLecturesFromTonnie()
        {
            Student tonnie = SqlDataContext.Students.First(student => student.name.Equals("Tonnie"));

            IEnumerable<Lecture> tonniesLectures = from sl in tonnie.Student_Lectures select sl.Lecture;

            MainDataGrid.ItemsSource = tonniesLectures;
        }

        public void GetAllStudentsFromYale()
        {
            IEnumerable<Student> studentsFromYale = from student in SqlDataContext.Students
                                                    where student.University.name == "Yale"
                                                    select student;

            MainDataGrid.ItemsSource = studentsFromYale;
        }

        public void GetAllUniversitiesWithTransgenders()
        {
            IEnumerable<University> transgenderUniversities = from student in SqlDataContext.Students
                                                              join university in SqlDataContext.Universities
                                                              on student.University equals university
                                                              where student.gender == "trans-gender"
                                                              select university;

            MainDataGrid.ItemsSource = transgenderUniversities;
        }

        public void GetAllLecturesFromFhOsnabrück()
        {
            IEnumerable<Lecture> lecturesOsna = from sl in SqlDataContext.Student_Lectures
                                                join student in SqlDataContext.Students
                                                on sl.Student equals student
                                                where student.University.name == "FH Osnabrück"
                                                select sl.Lecture;

            MainDataGrid.ItemsSource = lecturesOsna;
        }

        public void UpdateTonnie()
        {
            Student tonnie = SqlDataContext.Students.First(student => student.name.Equals("Tonnie"));
            tonnie.name = "Antonio";

            SqlDataContext.SubmitChanges();

            MainDataGrid.ItemsSource = SqlDataContext.Students;
        }

        public void DeleteWu()
        {
            Student wu = SqlDataContext.Students.First(student => student.name.Equals("Wu"));
            SqlDataContext.Students.DeleteOnSubmit(wu);
            SqlDataContext.SubmitChanges();

            MainDataGrid.ItemsSource = SqlDataContext.Students;

        }
    }
}
